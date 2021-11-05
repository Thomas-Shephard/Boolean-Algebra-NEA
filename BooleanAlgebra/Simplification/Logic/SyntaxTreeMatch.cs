namespace BooleanAlgebra.Simplification.Logic; 

public static class SyntaxTreeMatch {
    public static bool IsMatch(SyntaxItem syntaxTree, SyntaxItem patternMatch, ref Dictionary<string, List<SyntaxItem>> substituteItems, ref Dictionary<string, SyntaxItem> directSubstitutes) {
        if (syntaxTree.Value != patternMatch.Value)
            return false;

        List<SyntaxItem> syntaxTreeDaughterItems = syntaxTree.DaughterItems.ToList();
        List<SyntaxItem> patternMatchDaughterItems = patternMatch.DaughterItems.ToList();

        for (int i = patternMatchDaughterItems.Count - 1; i >= 0; i--) {
            if (patternMatchDaughterItems[i] is not Operand {IsGeneric: false} explicitOperand)
                continue;
            int explicitOperandIndex = syntaxTreeDaughterItems.IndexOf(explicitOperand);
            if (explicitOperandIndex == -1)
                return false;
            syntaxTreeDaughterItems.RemoveAt(explicitOperandIndex);
            patternMatchDaughterItems.RemoveAt(i);
        }

        Dictionary<string, List<SyntaxItem>> genericSyntaxItems = GetGenericSyntaxItems(patternMatchDaughterItems);

        Dictionary<string, List<SyntaxItem>> matches = new();
        foreach ((string genericSyntaxItemName, List<SyntaxItem> genericSyntaxItem) in genericSyntaxItems.Where(genericSyntaxItem => genericSyntaxItem.Key.StartsWith("Item") && !genericSyntaxItem.Key.StartsWith("Items"))) {
            matches.Add(genericSyntaxItemName, ValidSyntaxItems(genericSyntaxItem.GetItemCounts(), syntaxTreeDaughterItems));
        }

        if (matches.Any(match => match.Value.Count == 0))
            return false;

        //Update ref types
        directSubstitutes = ProvideGenericItemWithDistinctItem(matches);
        
        foreach (SyntaxItem patternMatchDaughterItem in patternMatchDaughterItems) {
            if (SyntaxTreeSubstitution.TrySubstituteSyntaxTree(patternMatchDaughterItem, new Dictionary<string, List<SyntaxItem>>(),
                    directSubstitutes, out SyntaxItem? currentValue)) {
                syntaxTreeDaughterItems.Remove(currentValue);
            } else {
                throw new Exception();
            }
        }

        if (!syntaxTreeDaughterItems.Any())
            return true;
        
        IEnumerable<RepeatingOperator> repeatingOperators = genericSyntaxItems.SelectMany(x => x.Value).OfType<RepeatingOperator>();
        foreach (RepeatingOperator repeatingOperator in repeatingOperators) {
            List<SyntaxItem> matchedItems = new();

            
            substituteItems.Add(repeatingOperator.DaughterItem.Value, syntaxTreeDaughterItems);
        }

        return syntaxTreeDaughterItems.Any();
    }
    
    


    private static List<SyntaxItem> ValidSyntaxItems(IDictionary<SyntaxItem, int> genericItemCounts, IEnumerable<SyntaxItem> remainingSyntaxItems, IDictionary<string, SyntaxItem>? previouslyMatchedItems = null) {
        remainingSyntaxItems = remainingSyntaxItems.ToList();
        if (genericItemCounts.Values.All(x => x == 0))
            return new List<SyntaxItem> { previouslyMatchedItems.First().Value }; 
        if (!remainingSyntaxItems.Any())
            return new List<SyntaxItem>();


        List<SyntaxItem> returnValue = new();
        
        foreach (SyntaxItem remainingSyntaxItem in remainingSyntaxItems) {
            List<SyntaxItem> newRemainingSyntaxItems = remainingSyntaxItems.ToList();
            newRemainingSyntaxItems.Remove(remainingSyntaxItem);
            
           foreach ((SyntaxItem? key, int value) in genericItemCounts.Where(genericItemCount => genericItemCount.Value > 0)) {
               if(!TryGetGenericOperandFromSyntaxTree(remainingSyntaxItem, key, out SyntaxItem? foundSyntaxItem, out string? identifiedName))
                   continue;
               Dictionary<string, SyntaxItem> newPreviouslyMatchedItems = previouslyMatchedItems is null 
                   ? new Dictionary<string, SyntaxItem>() 
                   : new Dictionary<string, SyntaxItem>(previouslyMatchedItems);
                
               Dictionary<SyntaxItem, int> newGenericItemCounts = new(genericItemCounts) {
                   [key] = value - 1
               };

               if (newPreviouslyMatchedItems.TryGetValue(identifiedName, out SyntaxItem? previouslyFoundSyntaxItem)) {
                   if(!foundSyntaxItem.Equals(previouslyFoundSyntaxItem))
                       continue;
               } else {
                   newPreviouslyMatchedItems.Add(identifiedName, foundSyntaxItem);
               }

               returnValue.AddRange(ValidSyntaxItems(newGenericItemCounts, newRemainingSyntaxItems, newPreviouslyMatchedItems));
           }
        }

        return returnValue;
    }




    private static bool TryGetGenericOperandFromSyntaxTree(SyntaxItem syntaxTree, SyntaxItem genericSyntaxTree, [NotNullWhen(true)] out SyntaxItem? genericItem, [NotNullWhen(true)] out string? genericItemName) {
        genericItem = default;
        genericItemName = default;
        
        if (genericSyntaxTree is Operand {IsGeneric: true}) {
            genericItem = syntaxTree;
            genericItemName = genericSyntaxTree.Value;
            return true;
        }

        if (syntaxTree.Value != genericSyntaxTree.Value) {
            return false;
        }

        if (syntaxTree.DaughterItems.Count != genericSyntaxTree.DaughterItems.Count) {
            return false;
        }
        
        for (int i = syntaxTree.DaughterItems.Count - 1; i >= 0; i--) {
            if (TryGetGenericOperandFromSyntaxTree(syntaxTree.DaughterItems[i], genericSyntaxTree.DaughterItems[i],
                    out genericItem, out genericItemName)) {
                return true;
            }
        }
        
        return false;
    }
    

    private static bool DoesSyntaxTreeContainOperand(SyntaxItem syntaxTree, Operand operand) {
        if (syntaxTree.Equals(operand))
            return true;
        return syntaxTree.DaughterItems.Any(daughterSyntaxItem => DoesSyntaxTreeContainOperand(daughterSyntaxItem, operand));
    }
    
    private static Dictionary<Operand, int> GetOperandCounts(IEnumerable<SyntaxItem> daughterSyntaxItems) {
        Dictionary<Operand, int> returnValue = new();

        foreach (SyntaxItem syntaxTreeDaughterItem in daughterSyntaxItems) {
            if (syntaxTreeDaughterItem is Operand currentOperand) {
                returnValue.TryGetValue(currentOperand, out int currentCount);
                returnValue[currentOperand] = currentCount + 1;
            } else {
                foreach ((Operand? key, int value) in GetOperandCounts(syntaxTreeDaughterItem.DaughterItems)) {
                    returnValue.TryGetValue(key, out int currentCount);
                    returnValue[key] = currentCount + value;
                }
            }
        }

        return returnValue;
    }

    private static Dictionary<string, SyntaxItem> ProvideGenericItemWithDistinctItem(Dictionary<string, List<SyntaxItem>> genericItems) {
        Dictionary<string, SyntaxItem> returnValue = new();
        List<SyntaxItem> usedSyntaxItems = new();
        
        foreach ((string? key, List<SyntaxItem>? value) in genericItems) {
            bool hasFoundMatch = false;
    
            foreach (SyntaxItem syntaxItem in value.Where(syntaxItem => !usedSyntaxItems.Contains(syntaxItem))) {
                usedSyntaxItems.Add(syntaxItem);
                returnValue.Add(key, syntaxItem);
                hasFoundMatch = true;
                break;
            }

            if(!hasFoundMatch)
                throw new Exception();
        }
        return returnValue;
    }

    private static List<SyntaxItem> GetDaughterSynItems(IEnumerable<SyntaxItem> daughterSyntaxItems, string genericItem) {
        return daughterSyntaxItems.Where(syntaxTreeDaughterItem =>
            DoesSyntaxTreeContainGenericItemName(syntaxTreeDaughterItem, genericItem)).ToList();
    }

    private static bool DoesSyntaxTreeContainGenericItemName(SyntaxItem syntaxTree, string genericItemName) {
        return syntaxTree.Value == genericItemName 
               || syntaxTree.DaughterItems.Any(syntaxTreeDaughterItem => DoesSyntaxTreeContainGenericItemName(syntaxTreeDaughterItem, genericItemName));
    }

    private static Dictionary<string, List<SyntaxItem>> GetGenericSyntaxItems(IEnumerable<SyntaxItem> daughterSyntaxItems) {
        daughterSyntaxItems = daughterSyntaxItems.ToArray();
        return GetGenericItemNames(daughterSyntaxItems)
            .ToDictionary(genericItem => genericItem, genericItem => GetDaughterSynItems(daughterSyntaxItems, genericItem));
    }
    
    private static IEnumerable<string> GetGenericItemNames(IEnumerable<SyntaxItem> daughterSyntaxItems, ICollection<string>? previouslyFoundDaughterItems = null) {
        previouslyFoundDaughterItems ??= new List<string>();
        List<string> returnValue = new();
        foreach (SyntaxItem syntaxTreeDaughterItem in daughterSyntaxItems) {
            if (syntaxTreeDaughterItem.DaughterItems.Count == 0) {
                if (!returnValue.Contains(syntaxTreeDaughterItem.Value) && !previouslyFoundDaughterItems.Contains(syntaxTreeDaughterItem.Value)){
                    returnValue.Add(syntaxTreeDaughterItem.Value);
                }
            } else {
                returnValue.AddRange(GetGenericItemNames(syntaxTreeDaughterItem.DaughterItems, returnValue));
            }
        }

        return returnValue;
    }
}