namespace BooleanAlgebra.Simplification.Logic;

public static class SyntaxTreeSubstitution {
    public static bool TrySubstituteSyntaxTree(SyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        if (!SyntaxTreeOnlyContainsKnownSubstitutes(syntaxTree, matches.DirectSubstitutes))
            throw new ArgumentException($"The parameter {nameof(matches)} did not contain a substitute for every generic operand.");
        if (!TryInternalSubstituteSyntaxTree(syntaxTree, matches.DirectSubstitutes, matches.RepeatingSubstitutes, out substitutedSyntaxTree))
            return false;
        substitutedSyntaxTree = substitutedSyntaxTree.Compress();
        return true;
    }

    private static bool SyntaxTreeOnlyContainsKnownSubstitutes(SyntaxItem syntaxTree, IReadOnlyDictionary<GenericOperand, SyntaxItem> directSubstitutes) {
        if (syntaxTree is not Operand operand)
            return syntaxTree.DaughterItems.All(daughterSyntaxItem => SyntaxTreeOnlyContainsKnownSubstitutes(daughterSyntaxItem, directSubstitutes));
        if (operand is not GenericOperand genericOperand) return true;
        return operand.Value.StartsWith("Items") || directSubstitutes.ContainsKey(genericOperand);
    }

    private static bool TryInternalSubstituteSyntaxTree(SyntaxItem syntaxTree, Dictionary<GenericOperand, SyntaxItem> directSubstitutes, IReadOnlyDictionary<GenericOperand, List<SyntaxItem>> repeatingSubstitutes, 
        [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is Operand operand)
            return TrySubstituteOperand(operand, directSubstitutes, out substitutedSyntaxTree);
        return TrySimplifyDaughterSyntaxItems(syntaxTree, directSubstitutes, repeatingSubstitutes, out substitutedSyntaxTree);
    }

    private static bool TrySubstituteOperand(SyntaxItem syntaxTree, IReadOnlyDictionary<GenericOperand, SyntaxItem> directSubstitutes,
        [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is GenericOperand genericOperand)
            return directSubstitutes.TryGetValue(genericOperand, out substitutedSyntaxTree);
        substitutedSyntaxTree = syntaxTree;
        return true;
    }

    private static bool TrySimplifyDaughterSyntaxItems(SyntaxItem syntaxTree, Dictionary<GenericOperand, SyntaxItem> directSubstitutes,
        IReadOnlyDictionary<GenericOperand, List<SyntaxItem>> repeatingSubstitutes, 
        [NotNullWhen(true)] out SyntaxItem? substitutedSyntaxTree) {
        syntaxTree = syntaxTree.Clone();
        for (int i = syntaxTree.DaughterItems.Count - 1; i >= 0; i--) {
            if (syntaxTree.DaughterItems[i] is RepeatingOperator repeatingOperator) {
                foreach (Dictionary<GenericOperand, SyntaxItem> tempDirectSubstitutes in GetDictionaries(repeatingOperator, directSubstitutes, repeatingSubstitutes)) {
                    if (TryInternalSubstituteSyntaxTree(repeatingOperator.DaughterItem, tempDirectSubstitutes, repeatingSubstitutes, out SyntaxItem? tempSubstitutedSyntaxTree)) {
                        syntaxTree.DaughterItems.Insert(i + 1, tempSubstitutedSyntaxTree);
                    } else {
                        substitutedSyntaxTree = default;
                        return false;
                    }
                }

                syntaxTree.DaughterItems.RemoveAt(i);
            } else if (TryInternalSubstituteSyntaxTree(syntaxTree.DaughterItems[i], directSubstitutes, repeatingSubstitutes, out SyntaxItem? tempSubstitutedSyntaxTree)) {
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

    private static IEnumerable<Dictionary<GenericOperand, SyntaxItem>> GetDictionaries(RepeatingOperator repeatingOperator,
        IDictionary<GenericOperand, SyntaxItem> directSubstitutes, IReadOnlyDictionary<GenericOperand, List<SyntaxItem>> repeatingSubstitutes) {
        return from repeatingOperatorName in
                GetRepeatingOperatorNames(repeatingOperator).Where(repeatingSubstitutes.ContainsKey).ToArray()
            from substituteSyntaxItem in repeatingSubstitutes[repeatingOperatorName].ToArray().Reverse()
            select new Dictionary<GenericOperand, SyntaxItem>(directSubstitutes)
                {{repeatingOperatorName, substituteSyntaxItem}};
    }

    private static IEnumerable<GenericOperand> GetRepeatingOperatorNames(RepeatingOperator repeatingOperator) {
        SyntaxItem daughterItem = repeatingOperator.DaughterItem;

        if (daughterItem.Value.StartsWith("Items") && daughterItem is GenericOperand genericOperand)
            return new[] {genericOperand};

        return daughterItem.DaughterItems.OfType<GenericOperand>().Where(genericOperandDaughterItem => genericOperandDaughterItem.Value.StartsWith("Items"));
    }
}