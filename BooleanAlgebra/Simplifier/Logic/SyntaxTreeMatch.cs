namespace BooleanAlgebra.Simplifier.Logic; 
public static class SyntaxTreeMatch {
    public static List<Matches> GetAllMatches(ISyntaxItem syntaxTree, ISyntaxItem patternMatchTree, Matches? inputMatches = null) {
        inputMatches ??= new Matches();

        if (!syntaxTree.IsIdentifierEqual(patternMatchTree)) return new List<Matches>();
        ISyntaxItem[] syntaxTreeDaughterItems = syntaxTree.GetChildNodes();
        ISyntaxItem[] patternMatchTreeDaughterItems = patternMatchTree.GetChildNodes();

        return GetNonRepeatingMatches(syntaxTreeDaughterItems, patternMatchTreeDaughterItems, inputMatches);
    }

    private static List<Matches> GetNonRepeatingMatches(IReadOnlyCollection<ISyntaxItem> syntaxTreeDaughterItems, IReadOnlyCollection<ISyntaxItem> patternMatchTreeDaughterItems, Matches inputMatches) {
        inputMatches = inputMatches;//.Clone();
        List<Matches> returnValue = new();
        List<SyntaxItemCountPair> syntaxTreeDaughterItemCounts = syntaxTreeDaughterItems.GetSyntaxItemCounts();
        List<SyntaxItemCountPair> patternMatchTreeDaughterItemCounts = patternMatchTreeDaughterItems.GetSyntaxItemCounts();

        foreach (ISyntaxItem syntaxTreeDaughterItem in syntaxTreeDaughterItemCounts
                     .Select(syntaxTreeDaughterItem => syntaxTreeDaughterItem.SyntaxItem)) {
            foreach (ISyntaxItem patternMatchTreeDaughterItem in patternMatchTreeDaughterItemCounts
                         .Select(patternMatchTreeDaughterItem => patternMatchTreeDaughterItem.SyntaxItem)
                         .Where(patternMatchTreeDaughterItem => patternMatchTreeDaughterItem is not RepeatingOperator)) {
                
                List<ISyntaxItem> syntaxTreeDaughterItemsCopy = syntaxTreeDaughterItems.ToList();
                syntaxTreeDaughterItemsCopy.Remove(syntaxTreeDaughterItem);
                List<ISyntaxItem> patternMatchTreeDaughterItemsCopy = patternMatchTreeDaughterItems.ToList();
                patternMatchTreeDaughterItemsCopy.Remove(patternMatchTreeDaughterItem);
                
                switch (patternMatchTreeDaughterItem) {
                    case GenericOperand genericOperand when inputMatches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out ISyntaxItem? foundDirectSubstitute):
                        if(foundDirectSubstitute.Equals(syntaxTreeDaughterItem)) {
                            returnValue.AddRange(GetNonRepeatingMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, inputMatches));
                        }
                        break;
                    case GenericOperand genericOperand:
                        Matches inputMatchesClone = inputMatches;//.Clone();
                        inputMatchesClone.DirectSubstitutes.Add(new DirectSubstitute(genericOperand, syntaxTreeDaughterItem));
                        returnValue.AddRange(GetNonRepeatingMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, inputMatchesClone));
                        break;
                    case Operand:
                        if (syntaxTreeDaughterItem.Equals(patternMatchTreeDaughterItem)) {
                            returnValue.AddRange(GetNonRepeatingMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, inputMatches));
                        }
                        break;
                    default:
                        foreach (Matches matches in GetAllMatches(syntaxTreeDaughterItem, patternMatchTreeDaughterItem, inputMatches)) {
                            returnValue.AddRange(GetNonRepeatingMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, matches));
                        }
                        break;
                }
            }
        }

        if(syntaxTreeDaughterItems.GetSyntaxItemCounts().All(x => x.Count == 0) && patternMatchTreeDaughterItems.GetSyntaxItemCounts().All(x => x.SyntaxItem is RepeatingOperator || x.Count == 0))
            returnValue.Add(inputMatches); 
        return returnValue.Count > 0 
            ? returnValue 
            : GetRepeatingMatches(syntaxTreeDaughterItemCounts, patternMatchTreeDaughterItemCounts, inputMatches);
    }

    private static List<Matches> GetRepeatingMatches(List<SyntaxItemCountPair> syntaxTreeDaughterItemCounts, List<SyntaxItemCountPair> patternMatchTreeDaughterItemCounts, Matches inputMatches) {
        List<Matches> returnValue = new();
        List<SyntaxItemCountPair> patternMatchTreeDaughterRemainingItems = patternMatchTreeDaughterItemCounts.Where(x => x.Count > 0).ToList();
        if (patternMatchTreeDaughterRemainingItems.Any(x => x.Count > 1 || x.SyntaxItem is not RepeatingOperator))
            return new List<Matches>();
        RepeatingOperator[] repeatingOperators = patternMatchTreeDaughterRemainingItems.Select(x => x.SyntaxItem).OfType<RepeatingOperator>().ToArray();
        foreach (RepeatingOperator repeatingOperator in repeatingOperators) {
            int number = TryGetRepeatingGenericOperandFromRepeatingOperator(repeatingOperator, out GenericOperand? foundGenericOperand);
 
            List<ISyntaxItem> matchedSyntaxItems = new();
                
            for (int i = syntaxTreeDaughterItemCounts.Count - 1; i >= 0; i--) {
                (ISyntaxItem syntaxTreeDaughterItem, int syntaxTreeDaughterCount) = syntaxTreeDaughterItemCounts.ElementAt(i);
                List<Matches> possibleMatches = number == 1
                    ? GetAllMatches(syntaxTreeDaughterItem, repeatingOperator.Child, inputMatches)
                    : GetNonRepeatingMatches(new[] { syntaxTreeDaughterItem }, new[] { repeatingOperator.Child }, inputMatches);
                if (possibleMatches.Count == 0) continue;
                syntaxTreeDaughterItemCounts.RemoveAll(x => x.SyntaxItem.Equals(syntaxTreeDaughterItem));

                if (!possibleMatches.First().TryGetDirectSubstituteFromGenericOperand(foundGenericOperand, out ISyntaxItem? substitute))
                    throw new Exception();
                
                matchedSyntaxItems.AddRange(Enumerable.Repeat(substitute, syntaxTreeDaughterCount));
            }

            if(matchedSyntaxItems.Count == 0)
                continue;
                
            if (inputMatches.TryGetRepeatingSubstituteFromGenericOperand(foundGenericOperand, out List<ISyntaxItem>? foundSyntaxItems)) {
                foundSyntaxItems.AddRange(matchedSyntaxItems);
            } else {
                inputMatches.RepeatingSubstitutes.Add(new RepeatingSubstitute(foundGenericOperand, matchedSyntaxItems));
            }
                
            returnValue.Add(inputMatches);
        }
            
        return syntaxTreeDaughterItemCounts.Any(x => x.Count > 0) ? new List<Matches>() : returnValue;
    }
    
    private static int TryGetRepeatingGenericOperandFromRepeatingOperator(RepeatingOperator repeatingOperator, out GenericOperand foundGenericOperand) {
        ISyntaxItem daughterItem = repeatingOperator.Child;
        
        if(daughterItem is GenericOperand {IsRepeating: true} repeatingGenericOperand) {
            foundGenericOperand = repeatingGenericOperand;
            return 0;
        }

        GenericOperand[] repeatingGenericOperands = daughterItem.GetChildNodes()
            .OfType<GenericOperand>()
            .Where(genericOperand => genericOperand.IsRepeating)
            .ToArray();

        if (repeatingGenericOperands.Length != 1) {
            throw new Exception();
        }

        foundGenericOperand = repeatingGenericOperands.First();
        return 1;
    }
}