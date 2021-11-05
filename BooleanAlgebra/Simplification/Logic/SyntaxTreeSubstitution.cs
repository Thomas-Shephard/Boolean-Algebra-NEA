namespace BooleanAlgebra.Simplification.Logic;

public static class SyntaxTreeSubstitution {
    public static bool TrySubstituteSyntaxTree(SyntaxItem syntaxTree,
        Dictionary<string, List<SyntaxItem>> substituteItems, Dictionary<string, SyntaxItem> directSubstitutes,
        [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        if (!SyntaxTreeOnlyContainsKnownSubstitutes(syntaxTree, directSubstitutes))
            throw new ArgumentException(
                $"The parameter {nameof(directSubstitutes)} did not contain a substitute for every generic operand.");
        if (!TryInternalSubstituteSyntaxTree(syntaxTree, substituteItems, directSubstitutes, out substitutedSyntaxTree)) return false;
        //substitutedSyntaxTree = substitutedSyntaxTree.Clone();
        return true;

    }

    private static bool SyntaxTreeOnlyContainsKnownSubstitutes(SyntaxItem syntaxTree,
        IReadOnlyDictionary<string, SyntaxItem> directSubstitutes) {
        if (syntaxTree is not Operand operand)
            return syntaxTree.DaughterItems.All(daughterSyntaxItem =>
                SyntaxTreeOnlyContainsKnownSubstitutes(daughterSyntaxItem, directSubstitutes));
        if (!operand.IsGeneric) return true;
        return operand.Value.StartsWith("Items") || directSubstitutes.ContainsKey(operand.Value);
    }

    private static bool TryInternalSubstituteSyntaxTree(SyntaxItem syntaxTree,
        IReadOnlyDictionary<string, List<SyntaxItem>> substituteItems, Dictionary<string, SyntaxItem> directSubstitutes,
        [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is Operand operand)
            return TrySubstituteOperand(operand, directSubstitutes, out substitutedSyntaxTree);
        return TrySimplifyDaughterSyntaxItems(syntaxTree, substituteItems, directSubstitutes,
            out substitutedSyntaxTree);
    }

    private static bool TrySubstituteOperand(Operand operand, IReadOnlyDictionary<string, SyntaxItem> directSubstitutes,
        [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        if (operand.IsGeneric)
            return directSubstitutes.TryGetValue(operand.Value, out substitutedSyntaxTree);

        substitutedSyntaxTree = operand;
        return true;
    }

    private static bool TrySimplifyDaughterSyntaxItems(SyntaxItem syntaxTree,
        IReadOnlyDictionary<string, List<SyntaxItem>> substituteItems, Dictionary<string, SyntaxItem> directSubstitutes,
        [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        syntaxTree = syntaxTree.Clone();
        for (int i = syntaxTree.DaughterItems.Count - 1; i >= 0; i--) {
            if (syntaxTree.DaughterItems[i] is RepeatingOperator repeatingOperator) {
                foreach (Dictionary<string, SyntaxItem> tempDirectSubstitutes in GetDictionaries(repeatingOperator, substituteItems, directSubstitutes)) {
                    if (TryInternalSubstituteSyntaxTree(repeatingOperator.DaughterItem, substituteItems,
                            tempDirectSubstitutes,
                            out SyntaxItem? tempSubstitutedSyntaxTree)) {
                        syntaxTree.DaughterItems.Insert(i + 1, tempSubstitutedSyntaxTree);
                    } else {
                        substitutedSyntaxTree = default;
                        return false;
                    }
                }

                syntaxTree.DaughterItems.RemoveAt(i);
            } else if (TryInternalSubstituteSyntaxTree(syntaxTree.DaughterItems[i], substituteItems, directSubstitutes,
                           out SyntaxItem? tempSubstitutedSyntaxTree)) {
                syntaxTree.DaughterItems[i] = tempSubstitutedSyntaxTree;
            } else {
                syntaxTree.DaughterItems.RemoveAt(i);
            }
        }

        if (syntaxTree is not BinaryOperator {DaughterItems.Count: <= 1}) {
            substitutedSyntaxTree = syntaxTree;
            return true;
        }

        if (syntaxTree.DaughterItems.Count == 1) {
            substitutedSyntaxTree = syntaxTree.DaughterItems.First();
            return true;
        }

        substitutedSyntaxTree = default;
        return false;
    }

    private static IEnumerable<Dictionary<string, SyntaxItem>> GetDictionaries(RepeatingOperator repeatingOperator, IReadOnlyDictionary<string, List<SyntaxItem>> substituteItems, IDictionary<string, SyntaxItem> directSubstitutes) {
        return from repeatingOperatorName in
                GetRepeatingOperatorNames(repeatingOperator).Where(substituteItems.ContainsKey).ToArray()
            from substituteSyntaxItem in substituteItems[repeatingOperatorName].ToArray().Reverse()
            select new Dictionary<string, SyntaxItem>(directSubstitutes)
                {{repeatingOperatorName, substituteSyntaxItem}};
    }
    
    private static IEnumerable<string> GetRepeatingOperatorNames(RepeatingOperator repeatingOperator) {
        SyntaxItem daughterItem = repeatingOperator.DaughterItem;

        if (daughterItem.Value.StartsWith("Items")) {
            return new[] {daughterItem.Value};
        }

        return daughterItem.DaughterItems
            .Where(item => item.Value.StartsWith("Items"))
            .Select(x => x.Value);
    }
}