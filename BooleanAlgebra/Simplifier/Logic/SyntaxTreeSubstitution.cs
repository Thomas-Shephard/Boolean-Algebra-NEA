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
        if (!SyntaxTreeOnlyContainsKnownSubstitutes(syntaxTree, matches))
            throw new ArgumentException($"The parameter {nameof(matches)} did not contain a substitute for every generic operand.");
        //Attempt to substitute the syntax tree, if it fails then return false.
        if (!TryInternalSubstituteSyntaxTree(syntaxTree, matches, out substitutedSyntaxTree))
            return false;
        //If the syntax tree was substituted, compress the syntax tree and return true.
        //Compressing the syntax tree will remove redundant operators (e.g. ((A AND B) AND C) --> (A AND B AND C))
        substitutedSyntaxTree = substitutedSyntaxTree.Compress();
        return true;
    }

    private static bool SyntaxTreeOnlyContainsKnownSubstitutes(ISyntaxItem syntaxTree, Matches matches) {
        if (syntaxTree is not Operand operand)
            return syntaxTree.GetChildNodes().All(daughterSyntaxItem => SyntaxTreeOnlyContainsKnownSubstitutes(daughterSyntaxItem, matches));
        if (operand is not GenericOperand genericOperand) return true;
        return operand.Value.StartsWith("Items") || matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out _);
    }
    
    private static bool TryInternalSubstituteSyntaxTree(ISyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is Operand operand)
            return TrySubstituteOperand(operand, matches, out substitutedSyntaxTree);
        return TrySimplifyDaughterSyntaxItems(syntaxTree, matches, out substitutedSyntaxTree);
    }
    
    private static bool TrySubstituteOperand(ISyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is GenericOperand genericOperand)
            return matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out substitutedSyntaxTree);
        substitutedSyntaxTree = syntaxTree;
        return true;
    }

    private static bool TrySimplifyDaughterSyntaxItems(ISyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        List<ISyntaxItem> syntaxTreeDaughterItems = syntaxTree.GetChildNodes().ToList();
        
        for (int i = syntaxTreeDaughterItems.Count - 1; i >= 0; i--) {
            if (syntaxTreeDaughterItems[i] is RepeatingOperator repeatingOperator) {
                foreach (Matches tempMatch in GetDictionaries(repeatingOperator, matches)) {
                    if (TryInternalSubstituteSyntaxTree(repeatingOperator.Child, tempMatch, out ISyntaxItem? tempSubstitutedSyntaxTree)) {
                        syntaxTreeDaughterItems.Insert(i + 1, tempSubstitutedSyntaxTree);
                    } else {
                        substitutedSyntaxTree = default;
                        return false;
                    }
                }

                syntaxTreeDaughterItems.RemoveAt(i);
            } else if (TryInternalSubstituteSyntaxTree(syntaxTreeDaughterItems[i], matches, out ISyntaxItem? tempSubstitutedSyntaxTree)) {
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
    
    private static List<Matches> GetDictionaries(RepeatingOperator repeatingOperator, Matches matches) {
        List<Matches> matchesList = new();
        foreach (GenericOperand tempRepeatingOperator in GetRepeatingOperatorNames(repeatingOperator)) {
            if (!matches.TryGetRepeatingSubstituteFromGenericOperand(tempRepeatingOperator, out List<ISyntaxItem>? substitutes))
                continue;
            List<DirectSubstitute> directSubstitutes = substitutes.Select(substitute => new DirectSubstitute(tempRepeatingOperator, substitute)).ToList();
            matchesList.Add(new Matches(directSubstitutes, matches.RepeatingSubstitutes));
        }

        return matchesList;
    }

    private static IEnumerable<GenericOperand> GetRepeatingOperatorNames(RepeatingOperator repeatingOperator) {
        ISyntaxItem daughterItem = repeatingOperator.Child;

        if (daughterItem is GenericOperand { IsRepeating: true } genericOperand)
            return new[] {genericOperand};
        return daughterItem.GetChildNodes().OfType<GenericOperand>().Where(genericOperandDaughterItem => genericOperandDaughterItem.IsRepeating);
    }
}