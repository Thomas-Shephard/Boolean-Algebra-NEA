using BooleanAlgebra.Simplification;

namespace BooleanAlgebra.Simplifier.Logic; 

public static class SyntaxTreeMatch {
    public static List<Matches> GetAllMatches(SyntaxItem syntaxTree, SyntaxItem patternMatchTree, Matches? inputMatches = null) {
        inputMatches ??= new Matches();

        if (!syntaxTree.IsShallowMatch(patternMatchTree)) return new List<Matches>();
        List<SyntaxItem> syntaxTreeDaughterItems = syntaxTree.DaughterItems.ToList();
        List<SyntaxItem> patternMatchTreeDaughterItems = patternMatchTree.DaughterItems.ToList();

        return GetMatches(syntaxTreeDaughterItems, patternMatchTreeDaughterItems, inputMatches);
    }

    private static List<Matches> GetMatches(IReadOnlyCollection<SyntaxItem> syntaxTreeDaughterItems, IReadOnlyCollection<SyntaxItem> patternMatchTreeDaughterItems, Matches inputMatches) {
        List<Matches> returnValue = new();
        
        Dictionary<SyntaxItem, int> syntaxTreeDaughterItemCounts = syntaxTreeDaughterItems.GetItemCounts();
        Dictionary<SyntaxItem, int> patternMatchTreeDaughterItemCounts = patternMatchTreeDaughterItems.GetItemCounts();

        foreach (SyntaxItem syntaxTreeDaughterItem in syntaxTreeDaughterItemCounts
                     .Select(syntaxTreeDaughterItem => syntaxTreeDaughterItem.Key)) {
            foreach (SyntaxItem patternMatchTreeDaughterItem in patternMatchTreeDaughterItemCounts
                         .Where(patternMatchTreeDaughterItem => patternMatchTreeDaughterItem.Key is not RepeatingOperator)
                         .Select(patternMatchTreeDaughterItem => patternMatchTreeDaughterItem.Key)) {
                switch (patternMatchTreeDaughterItem) {
                    case GenericOperand genericOperand when inputMatches.DirectSubstitutes.TryGetValue(genericOperand, out SyntaxItem? foundDirectSubstitute): {
                        if(!foundDirectSubstitute.Equals(syntaxTreeDaughterItem)) {
                            continue;
                        }
                        
                        List<SyntaxItem> syntaxTreeDaughterItemsCopy = syntaxTreeDaughterItems.ToList();
                        syntaxTreeDaughterItemsCopy.Remove(syntaxTreeDaughterItem);
                        List<SyntaxItem> patternMatchTreeDaughterItemsCopy = patternMatchTreeDaughterItems.ToList();
                        patternMatchTreeDaughterItemsCopy.Remove(patternMatchTreeDaughterItem);
                        
                        returnValue.AddRange(GetMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, inputMatches));
                        break;
                    }
                    case GenericOperand genericOperand: {
                        List<SyntaxItem> syntaxTreeDaughterItemsCopy = syntaxTreeDaughterItems.ToList();
                        syntaxTreeDaughterItemsCopy.Remove(syntaxTreeDaughterItem);
                        List<SyntaxItem> patternMatchTreeDaughterItemsCopy = patternMatchTreeDaughterItems.ToList();
                        patternMatchTreeDaughterItemsCopy.Remove(patternMatchTreeDaughterItem);
                        
                        Matches inputMatchesClone = inputMatches.Clone();
                        inputMatchesClone.DirectSubstitutes.Add(genericOperand, syntaxTreeDaughterItem);

                        returnValue.AddRange(GetMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, inputMatchesClone));
                        break;
                    }
                    case Operand: {
                        if (!syntaxTreeDaughterItem.Equals(patternMatchTreeDaughterItem)) {
                            continue;
                        }
                        
                        List<SyntaxItem> syntaxTreeDaughterItemsCopy = syntaxTreeDaughterItems.ToList();
                        syntaxTreeDaughterItemsCopy.Remove(patternMatchTreeDaughterItem);
                        List<SyntaxItem> patternMatchTreeDaughterItemsCopy = patternMatchTreeDaughterItems.ToList();
                        patternMatchTreeDaughterItemsCopy.Remove(patternMatchTreeDaughterItem);
                        
                        returnValue.AddRange(GetMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, inputMatches));
                        break;
                    }
                    default: {
                        List<SyntaxItem> syntaxTreeDaughterItemsCopy = syntaxTreeDaughterItems.ToList();
                        syntaxTreeDaughterItemsCopy.Remove(syntaxTreeDaughterItem);
                        List<SyntaxItem> patternMatchTreeDaughterItemsCopy = patternMatchTreeDaughterItems.ToList();
                        patternMatchTreeDaughterItemsCopy.Remove(patternMatchTreeDaughterItem);
                        foreach (Matches matches in GetAllMatches(syntaxTreeDaughterItem, patternMatchTreeDaughterItem, inputMatches)) {
                            returnValue.AddRange(GetMatches(syntaxTreeDaughterItemsCopy, patternMatchTreeDaughterItemsCopy, matches));
                        }
                        break;
                    }
                }
            }
        }
        
        if (returnValue.Count != 0) return returnValue;
        if(syntaxTreeDaughterItemCounts.All(x => x.Value == 0) && patternMatchTreeDaughterItemCounts.Where(x => x.Key is not RepeatingOperator).All(x => x.Value == 0)) {
            returnValue.Add(inputMatches);
        } else {
            KeyValuePair<SyntaxItem, int>[] patternMatchTreeDaughterRemainingItems = patternMatchTreeDaughterItemCounts.Where(x => x.Value > 0).ToArray();
            if (patternMatchTreeDaughterRemainingItems.Any(x => x.Value > 1 || x.Key is not RepeatingOperator))
                return new List<Matches>();
            RepeatingOperator[] repeatingOperators = patternMatchTreeDaughterRemainingItems.Select(x => x.Key).OfType<RepeatingOperator>().ToArray();
            foreach (RepeatingOperator repeatingOperator in repeatingOperators) {
                int number = TryGetRepeatingGenericOperandFromRepeatingOperator(repeatingOperator, out GenericOperand? foundGenericOperand);
                
                if (number == -1) {
                    throw new Exception();
                }

                List<SyntaxItem> matchedSyntaxItems = new();
                
                for (int i = syntaxTreeDaughterItemCounts.Count - 1; i >= 0; i--) {
                    (SyntaxItem syntaxTreeDaughterItem, int syntaxTreeDaughterCount) = syntaxTreeDaughterItemCounts.ElementAt(i);
                    List<Matches> possibleMatches = number == 1
                        ? GetAllMatches(syntaxTreeDaughterItem, repeatingOperator.DaughterItem, inputMatches)
                        : GetMatches(new[] { syntaxTreeDaughterItem }, new[] { repeatingOperator.DaughterItem }, inputMatches);
                    if (possibleMatches.Count == 0) continue;
                    syntaxTreeDaughterItemCounts.Remove(syntaxTreeDaughterItem);
                    matchedSyntaxItems.AddRange(Enumerable.Repeat(possibleMatches.First().DirectSubstitutes[foundGenericOperand], syntaxTreeDaughterCount));
                }

                if(matchedSyntaxItems.Count == 0)
                    continue;
                
                if (inputMatches.RepeatingSubstitutes.TryGetValue(foundGenericOperand, out List<SyntaxItem>? foundSyntaxItems)) {
                    foundSyntaxItems.AddRange(matchedSyntaxItems);
                } else {
                    inputMatches.RepeatingSubstitutes.Add(foundGenericOperand, matchedSyntaxItems);
                }
                
                returnValue.Add(inputMatches);
            }
            
            if (syntaxTreeDaughterItemCounts.Any(x => x.Value > 0)) {
                return new List<Matches>();
            }
        }

        return returnValue;
    }
    
    private static int TryGetRepeatingGenericOperandFromRepeatingOperator(RepeatingOperator repeatingOperator, out GenericOperand foundGenericOperand) {
        SyntaxItem daughterItem = repeatingOperator.DaughterItem;
        
        if(daughterItem is GenericOperand {IsRepeating: true} repeatingGenericOperand) {
            foundGenericOperand = repeatingGenericOperand;
            return 0;
        }

        GenericOperand[] repeatingGenericOperands = daughterItem.DaughterItems
            .OfType<GenericOperand>()
            .Where(genericOperand => genericOperand.IsRepeating)
            .ToArray();

        if (repeatingGenericOperands.Length != 1) {
            foundGenericOperand = default;
            return -1;
        }

        foundGenericOperand = repeatingGenericOperands.First();
        return 1;
    }
}