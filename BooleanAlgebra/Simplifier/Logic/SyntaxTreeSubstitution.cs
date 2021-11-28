namespace BooleanAlgebra.Simplifier.Logic;

public static class SyntaxTreeSubstitution {
    public static bool TrySubstituteSyntaxTree(ISyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (!SyntaxTreeOnlyContainsKnownSubstitutes(syntaxTree, matches.DirectSubstitutes))
            throw new ArgumentException($"The parameter {nameof(matches)} did not contain a substitute for every generic operand.");
        if (!TryInternalSubstituteSyntaxTree(syntaxTree, matches.DirectSubstitutes, matches.RepeatingSubstitutes, out substitutedSyntaxTree))
            return false;
        substitutedSyntaxTree = substitutedSyntaxTree.Compress();
        return true;
    }

    private static bool SyntaxTreeOnlyContainsKnownSubstitutes(ISyntaxItem syntaxTree, IReadOnlyDictionary<GenericOperand, ISyntaxItem> directSubstitutes) {
        if (syntaxTree is not Operand operand)
            return syntaxTree.GetDaughterItems().All(daughterSyntaxItem => SyntaxTreeOnlyContainsKnownSubstitutes(daughterSyntaxItem, directSubstitutes));
        if (operand is not GenericOperand genericOperand) return true;
        return operand.Value.StartsWith("Items") || directSubstitutes.ContainsKey(genericOperand);
    }

    private static bool TryInternalSubstituteSyntaxTree(ISyntaxItem syntaxTree, Dictionary<GenericOperand, ISyntaxItem> directSubstitutes, IReadOnlyDictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes, 
        [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is Operand operand)
            return TrySubstituteOperand(operand, directSubstitutes, out substitutedSyntaxTree);
        return TrySimplifyDaughterSyntaxItems(syntaxTree, directSubstitutes, repeatingSubstitutes, out substitutedSyntaxTree);
    }

    private static bool TrySubstituteOperand(ISyntaxItem syntaxTree, IReadOnlyDictionary<GenericOperand, ISyntaxItem> directSubstitutes,
        [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is GenericOperand genericOperand)
            return directSubstitutes.TryGetValue(genericOperand, out substitutedSyntaxTree);
        substitutedSyntaxTree = syntaxTree;
        return true;
    }

    private static bool TrySimplifyDaughterSyntaxItems(ISyntaxItem syntaxTree, Dictionary<GenericOperand, ISyntaxItem> directSubstitutes,
        IReadOnlyDictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes, 
        [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        List<ISyntaxItem> syntaxTreeDaughterItems = syntaxTree.GetDaughterItems().ToList();
        
        for (int i = syntaxTreeDaughterItems.Count - 1; i >= 0; i--) {
            if (syntaxTreeDaughterItems[i] is RepeatingOperator repeatingOperator) {
                foreach (Dictionary<GenericOperand, ISyntaxItem> tempDirectSubstitutes in GetDictionaries(repeatingOperator, directSubstitutes, repeatingSubstitutes)) {
                    if (TryInternalSubstituteSyntaxTree(repeatingOperator.Daughter, tempDirectSubstitutes, repeatingSubstitutes, out ISyntaxItem? tempSubstitutedSyntaxTree)) {
                        syntaxTreeDaughterItems.Insert(i + 1, tempSubstitutedSyntaxTree);
                    } else {
                        substitutedSyntaxTree = default;
                        return false;
                    }
                }

                syntaxTreeDaughterItems.RemoveAt(i);
            } else if (TryInternalSubstituteSyntaxTree(syntaxTreeDaughterItems[i], directSubstitutes, repeatingSubstitutes, out ISyntaxItem? tempSubstitutedSyntaxTree)) {
                syntaxTreeDaughterItems[i] = tempSubstitutedSyntaxTree;
            } else {
                syntaxTreeDaughterItems.RemoveAt(i);
            }
        }

        switch (syntaxTreeDaughterItems.Count) {
            case > 1:
                substitutedSyntaxTree = new BinaryOperator(syntaxTree.Identifier, syntaxTreeDaughterItems.ToArray());
                return true;
            case 1:
                substitutedSyntaxTree = syntaxTree switch {
                    UnaryOperator => new UnaryOperator(syntaxTree.Identifier, syntaxTreeDaughterItems[0]),
                    _ => syntaxTreeDaughterItems[0]
                };
                return true;
            default:
                substitutedSyntaxTree = default;
                return false;
        }
    }

    private static IEnumerable<Dictionary<GenericOperand, ISyntaxItem>> GetDictionaries(RepeatingOperator repeatingOperator,
        IDictionary<GenericOperand, ISyntaxItem> directSubstitutes, IReadOnlyDictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes) {
        return from repeatingOperatorName in
                GetRepeatingOperatorNames(repeatingOperator).Where(repeatingSubstitutes.ContainsKey).ToArray()
            from substituteSyntaxItem in repeatingSubstitutes[repeatingOperatorName].ToArray().Reverse()
            select new Dictionary<GenericOperand, ISyntaxItem>(directSubstitutes)
                {{repeatingOperatorName, substituteSyntaxItem}};
    }

    private static IEnumerable<GenericOperand> GetRepeatingOperatorNames(RepeatingOperator repeatingOperator) {
        ISyntaxItem daughterItem = repeatingOperator.Daughter;

        if (daughterItem is GenericOperand genericOperand && genericOperand.Value.StartsWith("Items"))
            return new[] {genericOperand};

        return daughterItem.GetDaughterItems().OfType<GenericOperand>().Where(genericOperandDaughterItem => genericOperandDaughterItem.Value.StartsWith("Items"));
    }
}