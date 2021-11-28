namespace BooleanAlgebra.Simplifier.Logic; 
public static class SyntaxTreeMatch {
    public static List<Matches> GetAllMatches(ISyntaxItem syntaxTree, ISyntaxItem patternMatchTree, Matches? inputMatches = null) {
        inputMatches ??= new Matches();

        if (!syntaxTree.IsShallowMatch(patternMatchTree)) return new List<Matches>();
         ISyntaxItem[] syntaxTreeDaughterItems = syntaxTree.GetDaughterItems();
        ISyntaxItem[] patternMatchTreeDaughterItems = patternMatchTree.GetDaughterItems();

        return GetNonRepeatingMatches(syntaxTreeDaughterItems, patternMatchTreeDaughterItems, inputMatches);
    }

    private static List<Matches> GetNonRepeatingMatches(IReadOnlyCollection<ISyntaxItem> syntaxTreeDaughterItems, IReadOnlyCollection<ISyntaxItem> patternMatchTreeDaughterItems, Matches inputMatches) {
        inputMatches = inputMatches.Clone();
        List<Matches> returnValue = new();
        Dictionary<ISyntaxItem, int> syntaxTreeDaughterItemCounts = syntaxTreeDaughterItems.GetItemCounts();
        Dictionary<ISyntaxItem, int> patternMatchTreeDaughterItemCounts = patternMatchTreeDaughterItems.GetItemCounts();

        foreach (ISyntaxItem syntaxTreeDaughterItem in syntaxTreeDaughterItemCounts
                     .Select(syntaxTreeDaughterItem => syntaxTreeDaughterItem.Key)) {
            foreach (ISyntaxItem patternMatchTreeDaughterItem in patternMatchTreeDaughterItemCounts
                         .Select(patternMatchTreeDaughterItem => patternMatchTreeDaughterItem.Key)
                         .Where(patternMatchTreeDaughterItem => patternMatchTreeDaughterItem is not RepeatingOperator)) {
                
                List<ISyntaxItem> syntaxTreeDaughterItemsCopy = syntaxTreeDaughterItems.ToList();
                syntaxTreeDaughterItemsCopy.Remove(syntaxTreeDaughterItem);
                List<ISyntaxItem> patternMatchTreeDaughterItemsCopy = patternMatchTreeDaughterItems.ToList();
                patternMatchTreeDaughterItemsCopy.Remove(patternMatchTreeDaughterItem);
                
                switch (patternMatchTreeDaughterItem) {
                    case GenericOperand genericOperand when inputMatches.DirectSubstitutes.TryGetValue(genericOperand, out ISyntaxItem? foundDirectSubstitute):
                        if(foundDirectSubstitute.Equals(syntaxTreeDaughterItem)) {
                            returnValue.AddRange(GetNonRepeatingMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, inputMatches));
                        }
                        break;
                    case GenericOperand genericOperand:
                        Matches inputMatchesClone = inputMatches.Clone();
                        inputMatchesClone.DirectSubstitutes.Add(genericOperand, syntaxTreeDaughterItem);
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

        if(syntaxTreeDaughterItems.GetItemCounts().All(x => x.Value == 0) && patternMatchTreeDaughterItems.GetItemCounts().All(x => x.Key is RepeatingOperator || x.Value == 0))
            returnValue.Add(inputMatches); 
        return returnValue.Count > 0 
            ? returnValue 
            : GetRepeatingMatches(syntaxTreeDaughterItemCounts, patternMatchTreeDaughterItemCounts, inputMatches);
    }

    private static List<Matches> GetRepeatingMatches(Dictionary<ISyntaxItem, int> syntaxTreeDaughterItemCounts, Dictionary<ISyntaxItem, int> patternMatchTreeDaughterItemCounts, Matches inputMatches) {
        List<Matches> returnValue = new();
        KeyValuePair<ISyntaxItem, int>[] patternMatchTreeDaughterRemainingItems = patternMatchTreeDaughterItemCounts.Where(x => x.Value > 0).ToArray();
        if (patternMatchTreeDaughterRemainingItems.Any(x => x.Value > 1 || x.Key is not RepeatingOperator))
            return new List<Matches>();
        RepeatingOperator[] repeatingOperators = patternMatchTreeDaughterRemainingItems.Select(x => x.Key).OfType<RepeatingOperator>().ToArray();
        foreach (RepeatingOperator repeatingOperator in repeatingOperators) {
            int number = TryGetRepeatingGenericOperandFromRepeatingOperator(repeatingOperator, out GenericOperand? foundGenericOperand);
 
            List<ISyntaxItem> matchedSyntaxItems = new();
                
            for (int i = syntaxTreeDaughterItemCounts.Count - 1; i >= 0; i--) {
                (ISyntaxItem syntaxTreeDaughterItem, int syntaxTreeDaughterCount) = syntaxTreeDaughterItemCounts.ElementAt(i);
                List<Matches> possibleMatches = number == 1
                    ? GetAllMatches(syntaxTreeDaughterItem, repeatingOperator.Daughter, inputMatches)
                    : GetNonRepeatingMatches(new[] { syntaxTreeDaughterItem }, new[] { repeatingOperator.Daughter }, inputMatches);
                if (possibleMatches.Count == 0) continue;
                syntaxTreeDaughterItemCounts.Remove(syntaxTreeDaughterItem);
                    
                matchedSyntaxItems.AddRange(Enumerable.Repeat(possibleMatches.First().DirectSubstitutes[foundGenericOperand], syntaxTreeDaughterCount));
            }

            if(matchedSyntaxItems.Count == 0)
                continue;
                
            if (inputMatches.RepeatingSubstitutes.TryGetValue(foundGenericOperand, out List<ISyntaxItem>? foundSyntaxItems)) {
                foundSyntaxItems.AddRange(matchedSyntaxItems);
            } else {
                inputMatches.RepeatingSubstitutes.Add(foundGenericOperand, matchedSyntaxItems);
            }
                
            returnValue.Add(inputMatches);
        }
            
        return syntaxTreeDaughterItemCounts.Any(x => x.Value > 0) ? new List<Matches>() : returnValue;
    }
    
    private static int TryGetRepeatingGenericOperandFromRepeatingOperator(RepeatingOperator repeatingOperator, out GenericOperand foundGenericOperand) {
        ISyntaxItem daughterItem = repeatingOperator.Daughter;
        
        if(daughterItem is GenericOperand {IsRepeating: true} repeatingGenericOperand) {
            foundGenericOperand = repeatingGenericOperand;
            return 0;
        }

        GenericOperand[] repeatingGenericOperands = daughterItem.GetDaughterItems()
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