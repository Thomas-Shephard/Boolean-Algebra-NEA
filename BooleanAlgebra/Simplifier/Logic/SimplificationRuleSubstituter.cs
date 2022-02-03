namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// Provides a set of methods that allow a given syntax tree to be substituted with a given matches object.
/// </summary>
public static class SyntaxTreeSubstitution {
    /// <summary>
    /// Attempts to substitute a given syntax tree with a given set of substitutes within the given matches object.
    /// Returns true if if the substitution was successful, false otherwise.
    /// Provides the substitution result if successful, null otherwise.
    /// </summary>
    /// <param name="syntaxItem">The syntax tree to substitute with the given matches object.</param>
    /// <param name="matches">The matches to substitute into the places within the given syntax tree.</param>
    /// <param name="substitutedSyntaxTree">The syntax tree that results after substituting the given syntax item with the given matches. Null if a substitution was not possible.</param>
    /// <returns>True when the given syntax tree was successfully substituted with the given matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxItem"/> or <paramref name="matches"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <param name="matches"> does not contain a substitute for every required generic operand.</param></exception>
    public static bool TrySubstituteSyntaxTree(ISyntaxItem syntaxItem, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxItem is null) throw new ArgumentNullException(nameof(syntaxItem));
        if (matches is null) throw new ArgumentNullException(nameof(matches));
        //Ensure that the syntax tree that is being substituted has an equivalence for each match.
        if (!SubstituteExistsForAllGenericOperands(syntaxItem, matches))
            throw new ArgumentException($"The parameter {nameof(matches)} did not contain a substitute for every required generic operand.");
        //Attempt to substitute the syntax tree, if it fails then return false.
        if (!TrySubstituteChildNodes(syntaxItem, matches, out substitutedSyntaxTree))
            return false;
        //If the syntax tree was substituted, compress the syntax tree and return true.
        //Compressing the syntax tree will remove redundant operators (e.g. ((A AND B) AND C) --> (A AND B AND C))
        substitutedSyntaxTree.Compress();
        return true;                       
    }

    /// <summary>
    /// Determines whether a given syntax tree has a substitute for every required generic operand.
    /// </summary>
    /// <param name="syntaxItem">The syntax tree to search for required generic operands in.</param>
    /// <param name="matches">The substitutes to determine whether each required generic operand has an equivalence for.</param>
    /// <returns>True if all required generic operands have a substitute.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxItem"/> or <paramref name="matches"/> is null.</exception>
    private static bool SubstituteExistsForAllGenericOperands(ISyntaxItem syntaxItem, Matches matches) {
        if (syntaxItem is null) throw new ArgumentNullException(nameof(syntaxItem));
        if (matches is null) throw new ArgumentNullException(nameof(matches));
        if (syntaxItem is not Operand operand)
            //If the syntax tree is not an operand, then check its child nodes and ensure that they contain only known substitutes.
            return syntaxItem.GetChildNodes().All(childSyntaxItem => SubstituteExistsForAllGenericOperands(childSyntaxItem, matches));
        //If the syntax item is an operand but not generic operand then it is not substitutable.
        if (operand is not GenericOperand genericOperand) return true;
        //Repeating generic operands are optional and therefore a match does not need to be present.
        //If the generic operand is not repeating then it does require a substitute, if no match is present then return false.
        return genericOperand.IsRepeating || matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out _);
    }

    /// <summary>
    /// Attempts to substitute a given syntax tree with a given set of substitutes within the given matches object.
    /// Returns true if if the substitution was successful, false otherwise.
    /// Provides the substitution result if successful, null otherwise.
    /// </summary>
    /// <param name="syntaxItem">The syntax tree to substitute with the given matches object.</param>
    /// <param name="matches">The matches to substitute into the places within the given syntax tree.</param>
    /// <param name="substitutedSyntaxTree">The syntax tree that results after substituting the given syntax item with the given matches. Null if a substitution was not possible.</param>
    /// <returns>True when the given syntax tree was successfully substituted with the given matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxItem"/> or <paramref name="matches"/> is null.</exception>
    private static bool TrySubstituteChildNodes(ISyntaxItem syntaxItem, Matches matches, [NotNullWhen(true)] out ISyntaxItem? substitutedSyntaxTree) {
        if (syntaxItem is null) throw new ArgumentNullException(nameof(syntaxItem));
        if (matches is null) throw new ArgumentNullException(nameof(matches));
        //If the syntax item is an operand, it has no child nodes to substitute.
        if (syntaxItem is Operand) {
            //If the syntax item is an operand and a generic operand, it should be substituted.
            //The substituted syntax item is set to the generic operand's substitute if it exists, else null.
            //If a substitute is found return true, else return false.
            if (syntaxItem is GenericOperand genericOperand)
                return matches.TryGetDirectSubstituteFromGenericOperand(genericOperand, out substitutedSyntaxTree);
            //If the syntax item is an operand but not a generic operand, it cannot be substituted.
            //Therefore, the substituted syntax item is the same as the original syntax item.
            substitutedSyntaxTree = syntaxItem;
            return true;
        }
        
        //If the syntax item is not an operand, then iterate over the child nodes and try to substitute them.
        List<ISyntaxItem> childNodes = syntaxItem.GetChildNodes().ToList();
        for (int i = childNodes.Count - 1; i >= 0; i--) {
            //If the current child node is a repeating operator, then its child node should be substituted.
            if (childNodes[i] is RepeatingOperator repeatingOperator) {
                //Iterate over all the different matches that could be substituted for the repeating generic operand.
                foreach (Matches tempMatch in GetAllMatchesFromRepeatingOperator(repeatingOperator, matches)) {
                    //Check whether the repeating operator can be completely substituted with the current match.
                    if(TrySubstituteChildNodes(repeatingOperator.ChildNode, tempMatch, out ISyntaxItem? tempSubstitutedSyntaxItem)) {
                        //Only insert the substituted syntax item if it was successfully substituted.
                        childNodes.Add(tempSubstitutedSyntaxItem);
                    }
                }
                //Remove the repeating operator from the child nodes.
                //As the repeating operator is optional, it is not necessary to check whether it has been substituted.
                childNodes.RemoveAt(i);
            //If the current child is not a repeating operator, then it should be attempted to be substituted
            } else if (TrySubstituteChildNodes(childNodes[i], matches, out ISyntaxItem? tempSubstitutedSyntaxItem)) {
                //Only if the child node was successfully substituted, then insert the substituted syntax item in place of the original child node.
                childNodes[i] = tempSubstitutedSyntaxItem;
            //If the current child is not a repeating operator and was not successfully substituted, then the syntax item cannot be substituted.
            } else {
                //As the syntax item could not be substituted, return false.
                substitutedSyntaxTree = default;
                return false;
            }
        }
        
        //If all the child nodes were successfully substituted, then create a new syntax item from the substituted child nodes.
        switch (childNodes.Count) {
            //If the original syntax item was a multi child syntax item and there are more than one child nodes, then create a new multi child syntax item.
            case > 1 when syntaxItem is IMultiChildSyntaxItem multiChildSyntaxItem:
                //Create a clone of the original multi child syntax item.
                IMultiChildSyntaxItem newMultiChildSyntaxItem = (IMultiChildSyntaxItem)multiChildSyntaxItem.ShallowClone();
                //Set the child nodes of the new multi child syntax item to the substituted child nodes.
                newMultiChildSyntaxItem.ChildNodes = childNodes.ToArray();
                //Set the substituted syntax item to the new multi child syntax item and return true.
                substitutedSyntaxTree = newMultiChildSyntaxItem;
                return true;
            //If the original syntax item was a multi child syntax item but there is only one child node, then only return the substituted child node.
            case 1 when syntaxItem is IMultiChildSyntaxItem:
                //Set the substituted syntax item to the substituted child node and return true.
                substitutedSyntaxTree = childNodes[0];
                return true;
            //If the original syntax item was a single child syntax item and there is one child node, then return the substituted child node as a child of the original syntax item.
            case 1 when syntaxItem is ISingleChildSyntaxItem singleChildSyntaxItem:
                //Create a clone of the original single child syntax item.
                ISingleChildSyntaxItem newSingleChildSyntaxItem = (ISingleChildSyntaxItem)singleChildSyntaxItem.ShallowClone();
                //Set the child node of the new single child syntax item to the substituted child node.
                newSingleChildSyntaxItem.ChildNode = childNodes[0];
                //Set the substituted syntax item to the new single child syntax item and return true.
                substitutedSyntaxTree = newSingleChildSyntaxItem;
                return true;
            //If none of the above cases are true, then the syntax item cannot be substituted correctly.
            default:
                //As the syntax item could not be substituted correctly, return false.
                substitutedSyntaxTree = default;
                return false;
        }
    }

    /// <summary>
    /// Returns a list of matches that can then be substituted with the syntax tree that is nested within the provided repeating operator.
    /// </summary>
    /// <param name="repeatingOperator">The repeating operator that contains a repeating generic operand nested inside it.</param>
    /// <param name="matches">The matches that are being substituted into the places within the given syntax tree.</param>
    /// <returns>A list of matches that can then be substituted with the syntax tree that is nested within the provided repeating operator.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="repeatingOperator"/> or <paramref name="matches"/> is null.</exception>
    private static List<Matches> GetAllMatchesFromRepeatingOperator(RepeatingOperator repeatingOperator, Matches matches) {
        if (repeatingOperator is null) throw new ArgumentNullException(nameof(repeatingOperator));
        if (matches is null) throw new ArgumentNullException(nameof(matches));
        List<Matches> matchesList = new();
        //Get the repeating generic operand that is nested inside the repeating operator, the depth is irrelevant and therefore discarded.
        GenericOperand repeatingGenericOperand = repeatingOperator.GetRepeatingGenericOperand(out _);
        //If no repeating substitute is found that matches the repeating generic operand, then return an empty list.
        if (!matches.TryGetRepeatingSubstituteFromGenericOperand(repeatingGenericOperand, out List<ISyntaxItem>? substitutes))
            return matchesList;
        //For each substitute that matches the repeating generic operand, create a new matches object with the substitute added to the direct substitute list.
        foreach (ISyntaxItem syntaxItem in substitutes) {
            //Create a new list of direct substitutes that is a copy of the current matches object's direct substitute list.
            List<DirectSubstitute> tempDirectSubstitutes = matches.DirectSubstitutes.ToList();
            //Add the new substitute to the direct substitute list.
            tempDirectSubstitutes.Add(new DirectSubstitute(repeatingGenericOperand, syntaxItem));
            //Add a new matches object to the matches list with the new direct substitute list.
            matchesList.Add(new Matches(tempDirectSubstitutes, matches.RepeatingSubstitutes));
        }

        return matchesList;
    }
}