namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// 
/// </summary>
public static class SyntaxTreeSubstitution {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="matches"></param>
    /// <param name="substitutedSyntaxTree"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool TrySubstituteSyntaxTree(ISyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        //Ensure that the syntax tree that is being substituted has an equivalence for each match.
        if (!SyntaxTreeOnlyContainsKnownSubstitutes(syntaxTree, matches.DirectSubstitutes))
            throw new ArgumentException($"The parameter {nameof(matches)} did not contain a substitute for every generic operand.");
        //Attempt to substitute the syntax tree, if it fails then return false.
        if (!TryInternalSubstituteSyntaxTree(syntaxTree, matches.DirectSubstitutes, matches.RepeatingSubstitutes, out substitutedSyntaxTree))
            return false;
        //If the syntax tree was substituted, compress the syntax tree and return true.
        //Compressing the syntax tree will remove redundant operators (e.g. ((A AND B) AND C) --> (A AND B AND C))
        substitutedSyntaxTree = substitutedSyntaxTree.Compress();
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="directSubstitutes"></param>
    /// <returns></returns>
    private static bool SyntaxTreeOnlyContainsKnownSubstitutes(ISyntaxItem syntaxTree, IReadOnlyDictionary<GenericOperand, ISyntaxItem> directSubstitutes) {
        if (syntaxTree is not Operand operand)
            return syntaxTree.GetChildNodes().All(daughterSyntaxItem => SyntaxTreeOnlyContainsKnownSubstitutes(daughterSyntaxItem, directSubstitutes));
        if (operand is not GenericOperand genericOperand) return true;
        return operand.Value.StartsWith("Items") || directSubstitutes.ContainsKey(genericOperand);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="directSubstitutes"></param>
    /// <param name="repeatingSubstitutes"></param>
    /// <param name="substitutedSyntaxTree"></param>
    /// <returns></returns>
    private static bool TryInternalSubstituteSyntaxTree(ISyntaxItem syntaxTree, Dictionary<GenericOperand, ISyntaxItem> directSubstitutes, IReadOnlyDictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes, 
        [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is Operand operand)
            return TrySubstituteOperand(operand, directSubstitutes, out substitutedSyntaxTree);
        return TrySimplifyDaughterSyntaxItems(syntaxTree, directSubstitutes, repeatingSubstitutes, out substitutedSyntaxTree);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="directSubstitutes"></param>
    /// <param name="substitutedSyntaxTree"></param>
    /// <returns></returns>
    private static bool TrySubstituteOperand(ISyntaxItem syntaxTree, IReadOnlyDictionary<GenericOperand, ISyntaxItem> directSubstitutes,
        [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is GenericOperand genericOperand)
            return directSubstitutes.TryGetValue(genericOperand, out substitutedSyntaxTree);
        substitutedSyntaxTree = syntaxTree;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="directSubstitutes"></param>
    /// <param name="repeatingSubstitutes"></param>
    /// <param name="substitutedSyntaxTree"></param>
    /// <returns></returns>
    private static bool TrySimplifyDaughterSyntaxItems(ISyntaxItem syntaxTree, Dictionary<GenericOperand, ISyntaxItem> directSubstitutes,
        IReadOnlyDictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes, 
        [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        List<ISyntaxItem> syntaxTreeDaughterItems = syntaxTree.GetChildNodes().ToList();
        
        for (int i = syntaxTreeDaughterItems.Count - 1; i >= 0; i--) {
            if (syntaxTreeDaughterItems[i] is RepeatingOperator repeatingOperator) {
                foreach (Dictionary<GenericOperand, ISyntaxItem> tempDirectSubstitutes in GetDictionaries(repeatingOperator, directSubstitutes, repeatingSubstitutes)) {
                    if (TryInternalSubstituteSyntaxTree(repeatingOperator.Child, tempDirectSubstitutes, repeatingSubstitutes, out ISyntaxItem? tempSubstitutedSyntaxTree)) {
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
                substitutedSyntaxTree = new BinaryOperator(syntaxTree.Identifier, syntaxTreeDaughterItems.ToArray()).Compress();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repeatingOperator"></param>
    /// <param name="directSubstitutes"></param>
    /// <param name="repeatingSubstitutes"></param>
    /// <returns></returns>
    private static IEnumerable<Dictionary<GenericOperand, ISyntaxItem>> GetDictionaries(RepeatingOperator repeatingOperator,
        IDictionary<GenericOperand, ISyntaxItem> directSubstitutes, IReadOnlyDictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes) {
        return from repeatingOperatorName in
                GetRepeatingOperatorNames(repeatingOperator).Where(repeatingSubstitutes.ContainsKey).ToArray()
            from substituteSyntaxItem in repeatingSubstitutes[repeatingOperatorName].ToArray().Reverse()
            select new Dictionary<GenericOperand, ISyntaxItem>(directSubstitutes)
                {{repeatingOperatorName, substituteSyntaxItem}};
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repeatingOperator"></param>
    /// <returns></returns>
    private static IEnumerable<GenericOperand> GetRepeatingOperatorNames(RepeatingOperator repeatingOperator) {
        ISyntaxItem daughterItem = repeatingOperator.Child;

        if (daughterItem is GenericOperand genericOperand && genericOperand.Value.StartsWith("Items"))
            return new[] {genericOperand};

        return daughterItem.GetChildNodes().OfType<GenericOperand>().Where(genericOperandDaughterItem => genericOperandDaughterItem.Value.StartsWith("Items"));
    }
}