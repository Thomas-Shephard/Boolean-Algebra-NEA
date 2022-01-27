namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// 
/// </summary>
public static class SyntaxTreeSubstitution {
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
            //If the syntax tree is not an operand, then check its child nodes and ensure that they contain only known substitutes.
            return syntaxTree.GetChildNodes().All(daughterSyntaxItem => SyntaxTreeOnlyContainsKnownSubstitutes(daughterSyntaxItem, matches));
        //If the syntax item is an operand but not generic operand then it is not substitutable.
        if (operand is not GenericOperand genericOperand) return true;
        //Repeating generic operands are optional and therefore a match does not need to be present.
        //If the generic operand is not repeating then it does require a substitute, if no match is present then return false.
        return genericOperand.IsRepeating || matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out _);
    }
    
    private static bool TryInternalSubstituteSyntaxTree(ISyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxTree is Operand operand)
            //If the syntax tree is an operand, attempt to substitute it (only operands can be substituted).
            return TrySubstituteOperand(operand, matches, out substitutedSyntaxTree);
        //If the syntax tree is not an operand, then attempt to substitute its child nodes.
        return TrySimplifyDaughterSyntaxItems(syntaxTree, matches, out substitutedSyntaxTree);
    }
    
    private static bool TrySubstituteOperand(Operand operand, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (operand is GenericOperand genericOperand)
            //If the operand is a generic operand then substitute it for its equivalence.
            //If no match is present then return false.
            return matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out substitutedSyntaxTree);
        //If the operand is not a generic operand then it is not substitutable and therefore its equivalence is its self.
        substitutedSyntaxTree = operand;
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
        foreach (GenericOperand repeatingGenericOperand in GetRepeatingOperatorNames(repeatingOperator)) {
            if (!matches.TryGetRepeatingSubstituteFromGenericOperand(repeatingGenericOperand, out List<ISyntaxItem>? substitutes))
                continue;
            List<DirectSubstitute> directSubstitutes = substitutes.Select(substitute => new DirectSubstitute(repeatingGenericOperand, substitute)).ToList();
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