namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// 
/// </summary>
public static class SyntaxTreeSubstitution {
    public static bool TrySubstituteSyntaxTree(ISyntaxItem syntaxTree, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        //Ensure that the syntax tree that is being substituted has an equivalence for each match.
        if (!MatchExistsForAllGenericOperands(syntaxTree, matches))
            throw new ArgumentException($"The parameter {nameof(matches)} did not contain a substitute for every generic operand.");
        //Attempt to substitute the syntax tree, if it fails then return false.
        if (!TrySubstituteChildNodes(syntaxTree, matches, out substitutedSyntaxTree))
            return false;
        //If the syntax tree was substituted, compress the syntax tree and return true.
        //Compressing the syntax tree will remove redundant operators (e.g. ((A AND B) AND C) --> (A AND B AND C))
        substitutedSyntaxTree.Compress();
        return true;                       
    }

    private static bool MatchExistsForAllGenericOperands(ISyntaxItem syntaxTree, Matches matches) {
        if (syntaxTree is not Operand operand)
            //If the syntax tree is not an operand, then check its child nodes and ensure that they contain only known substitutes.
            return syntaxTree.GetChildNodes().All(daughterSyntaxItem => MatchExistsForAllGenericOperands(daughterSyntaxItem, matches));
        //If the syntax item is an operand but not generic operand then it is not substitutable.
        if (operand is not GenericOperand genericOperand) return true;
        //Repeating generic operands are optional and therefore a match does not need to be present.
        //If the generic operand is not repeating then it does require a substitute, if no match is present then return false.
        return genericOperand.IsRepeating || matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out _);
    }

    private static bool TrySubstituteChildNodes(ISyntaxItem syntaxItem, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxItem) {
        if (syntaxItem is Operand) {
            //If the syntax item is an operand but not a generic operand, it cannot be substituted.
            if (syntaxItem is GenericOperand genericOperand)
                return matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out substitutedSyntaxItem);
            substitutedSyntaxItem = syntaxItem;
            return true;
        }
        
        List<ISyntaxItem> childNodes = syntaxItem.GetChildNodes().ToList();
        
        for (int i = childNodes.Count - 1; i >= 0; i--) {
            if (childNodes[i] is RepeatingOperator repeatingOperator) {
                foreach (Matches tempMatch in GetDictionaries(repeatingOperator, matches)) {
                    if(TrySubstituteChildNodes(repeatingOperator.Child, tempMatch, out ISyntaxItem? tempSubstitutedSyntaxItem)) {
                        childNodes.Add(tempSubstitutedSyntaxItem);
                    }
                }
                childNodes.RemoveAt(i);
            } else if (TrySubstituteChildNodes(childNodes[i], matches, out ISyntaxItem? tempSubstitutedSyntaxItem)) {
                childNodes[i] = tempSubstitutedSyntaxItem;
            } else {
                childNodes.RemoveAt(i);
            }
        }
        
        switch (childNodes.Count) {
            case > 1 when syntaxItem is IMultiChildSyntaxItem multiChildSyntaxItem:
                IMultiChildSyntaxItem newMultiChildSyntaxItem = (IMultiChildSyntaxItem)multiChildSyntaxItem.ShallowClone();
                newMultiChildSyntaxItem.Children = childNodes.ToArray();
                substitutedSyntaxItem = newMultiChildSyntaxItem;
                return true;
            case 1 when syntaxItem is IMultiChildSyntaxItem:
                substitutedSyntaxItem = childNodes[0];
                return true;
            case 1 when syntaxItem is ISingleChildSyntaxItem singleChildSyntaxItem:
                ISingleChildSyntaxItem newSingleChildSyntaxItem = (ISingleChildSyntaxItem)singleChildSyntaxItem.ShallowClone();
                newSingleChildSyntaxItem.Child = childNodes[0];
                substitutedSyntaxItem = newSingleChildSyntaxItem;
                return true;
            default:
                substitutedSyntaxItem = default;
                return false;
        }
    }

    private static List<Matches> GetDictionaries(RepeatingOperator repeatingOperator, Matches matches) {
        List<Matches> matchesList = new();
        GenericOperand repeatingGenericOperand = GetRepeatingGenericOperandFromRepeatingOperator(repeatingOperator);
        if (!matches.TryGetRepeatingSubstituteFromGenericOperand(repeatingGenericOperand, out List<ISyntaxItem>? substitutes))
            return matchesList;
        foreach (ISyntaxItem syntaxItem in substitutes) {
            List<DirectSubstitute> tempDirectSubstitutes = matches.DirectSubstitutes.ToList();
            tempDirectSubstitutes.Add(new DirectSubstitute(repeatingGenericOperand, syntaxItem));
            matchesList.Add(new Matches(tempDirectSubstitutes, matches.RepeatingSubstitutes));
        }

        return matchesList;
    }

    private static GenericOperand GetRepeatingGenericOperandFromRepeatingOperator(RepeatingOperator repeatingOperator) {
        //The repeating generic operand can be directly nested within the repeating operator.
        //If this is the case, then the repeating generic operand should be returned.
        if(repeatingOperator.Child is GenericOperand { IsRepeating: true } repeatingGenericOperand)
            return repeatingGenericOperand;
        //Get all the generic operands nested inside the repeating operators child node.
        GenericOperand[] foundRepeatingGenericOperands = repeatingOperator.Child.GetChildNodes().OfType<GenericOperand>().Where(genericOperand => genericOperand.IsRepeating).ToArray();
        //There can only be one repeating generic operand within a repeating operator.
        if(foundRepeatingGenericOperands.Length != 1)
            throw new InvalidOperationException("Repeating operators must have exactly one repeating generic operand nested inside of them.");
        //If there is only one repeating generic operand, then return it.
        return foundRepeatingGenericOperands.First();
    }
}