namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// Provides a method to allow a syntax tree to be simplified by applying the laws of boolean algebra.
/// </summary>
public static class AttemptSimplification {
    /// <summary>
    /// Returns a list of simplifications and their associated reasons that a given syntax tree can be substituted for.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree that should be simplified.</param>
    /// <param name="isPostSimplification">Whether the simplification rules applied should be those with the post simplification flag.</param>
    /// <returns>A list of simplifications and their associated reasons that the syntax tree can be substituted for.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="syntaxTree"/> is null.</exception>
    public static List<SimplificationRulePair> SimplifySyntaxTree(this ISyntaxItem syntaxTree, bool isPostSimplification) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        //Only use the post-simplification rules if the isPostSimplification flag is set to true.
        //Otherwise, use the non post-simplification rules.
        //Sort the rules by priority (lowest to highest) and group them together so that the rules are applied in the correct order.
        IEnumerable<IGrouping<int, SimplificationRule>> groupedSimplificationRulesCollectionByPrecedence = SimplificationRule.GetSimplificationRules()
           .Where(simplificationRule => simplificationRule.IsPostSimplification == isPostSimplification)
           .OrderBy(simplificationRule => simplificationRule.Precedence)
           .GroupBy(simplificationRule => simplificationRule.Precedence);
        //Iterate through the grouped rules and attempt to apply them to the syntax tree.
        foreach (IGrouping<int, SimplificationRule> groupedSimplificationRulesByPrecedence in groupedSimplificationRulesCollectionByPrecedence) {
            //Get the possible simplifications that can arise from the current collection of rules being applied.
            List<SimplificationRulePair> simplifications = syntaxTree.SimplifySyntaxTreeWithSimplificationRules(groupedSimplificationRulesByPrecedence.ToArray());
            //If any simplification was made, return the list of simplifications.
            if (simplifications.Count > 0)
                return simplifications;
        }

        //If no simplification was made, return an empty list.
        return new List<SimplificationRulePair>();
    }

    /// <summary>
    /// Returns a list of simplifications and their associated reasons that a given syntax tree can be substituted for.
    /// This is based off the simplification rules that are provided.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree that is being simplified.</param>
    /// <param name="simplificationRules">The simplification rules that can be applied to the given syntax tree.</param>
    /// <returns>A list of simplifications and their associated reasons that the syntax tree can be substituted for.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxTree"/> or <paramref name="simplificationRules"/> is null.</exception>
    private static List<SimplificationRulePair> SimplifySyntaxTreeWithSimplificationRules(this ISyntaxItem syntaxTree, SimplificationRule[] simplificationRules) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRules is null) throw new ArgumentNullException(nameof(simplificationRules));
        //All the simplifications at the same precedence should have the same simplification traversal order.
        //Therefore, getting the traversal order from the first rule in the list is sufficient.
        if (simplificationRules[0].SyntaxTreeTraversalOrder is SimplificationRuleTraversalOrder.INSIDE_OUT) {
            //If the traversal order is INSIDE_OUT, then the simplification rules should be applied to the child nodes of the current syntax item.
            List<SimplificationRulePair> simplifications = syntaxTree.SimplifySyntaxTreeChildNodesWithSimplificationRules(simplificationRules);
            //If any simplifications are possible, return the list of simplifications.
            //Otherwise, attempt to simplify the current syntax item with the simplification rules.
            return simplifications.Count > 0 
                ? simplifications 
                : syntaxTree.GetSimplificationsForSyntaxTreeFromSimplificationRules(simplificationRules);
        } else {
            //If the traversal order is not INSIDE_OUT, it must be OUTSIDE_IN.
            //In this case, the simplification rules should be applied to the current syntax item.
            List<SimplificationRulePair> simplifications = syntaxTree.GetSimplificationsForSyntaxTreeFromSimplificationRules(simplificationRules);
            //If any simplifications are possible, return the list of simplifications.
            //Otherwise, attempt to simplify the child nodes of the current syntax item with the simplification rules.
            return simplifications.Count > 0 
                ? simplifications 
                : syntaxTree.SimplifySyntaxTreeChildNodesWithSimplificationRules(simplificationRules);
        }
    }

    /// <summary>
    /// Simplifies the child nodes of a given syntax tree and provides the simplified version in the place of the simplified child node.
    /// This is based off the simplification rules that are provided.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree that is being simplified.</param>
    /// <param name="simplificationRules">The simplification rules that can be applied to the given syntax tree.</param>
    /// <returns>A list of simplifications and their associated reasons that a given syntax tree can be converted into.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxTree"/> or <paramref name="simplificationRules"/> is null.</exception>
    private static List<SimplificationRulePair> SimplifySyntaxTreeChildNodesWithSimplificationRules(this ISyntaxItem syntaxTree, SimplificationRule[] simplificationRules) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRules is null) throw new ArgumentNullException(nameof(simplificationRules));
        List<SimplificationRulePair> simplifications = new();
        //Simplifying a syntax tree is different depending on the type of the syntax tree.
        switch (syntaxTree) {
            //If the syntax tree has a single child node, then the simplification rules should be applied to the child node.
            case ISingleChildSyntaxItem singleChildSyntaxItem: {
                //Get all the possible simplifications that can arise from simplifying the child node of the current syntax item.
                List<SimplificationRulePair> childNodeSimplifications = singleChildSyntaxItem.ChildNode.SimplifySyntaxTreeWithSimplificationRules(simplificationRules);
                //Iterate over all of the possible simplifications that can arise from simplifying the child node of the current syntax item.
                foreach ((ISyntaxItem simplifiedSyntaxItem, string reason) in childNodeSimplifications) {
                    //Create a shallow clone of the current syntax item.
                    ISingleChildSyntaxItem newSingleChildSyntaxItem = (ISingleChildSyntaxItem) singleChildSyntaxItem.ShallowClone();
                    //Set the child node of the new syntax item to the simplified syntax item.
                    newSingleChildSyntaxItem.ChildNode = simplifiedSyntaxItem;
                    //Add the new syntax item to the list of simplifications.
                    simplifications.Add(new SimplificationRulePair(newSingleChildSyntaxItem, reason));
                }
                break;
            }
            //If the syntax tree has multiple child nodes, then the simplification rules should be applied to each child node.
            case IMultiChildSyntaxItem multiChildSyntaxItem: {
                //Iterate over all the child nodes of the current syntax item.
                for (int i = 0; i < multiChildSyntaxItem.ChildNodes.Length; i++) {
                    //Get all the possible simplifications that can arise from simplifying the current child node of the current syntax item.
                    List<SimplificationRulePair> childNodeSimplifications = multiChildSyntaxItem.ChildNodes[i].SimplifySyntaxTreeWithSimplificationRules(simplificationRules);
                    //Iterate over all of the possible simplifications that can arise from simplifying the current child node of the current syntax item.
                    foreach ((ISyntaxItem simplifiedSyntaxItem, string reason) in childNodeSimplifications) {
                        //Create a shallow clone of the current syntax item.
                        IMultiChildSyntaxItem newMultiChildSyntaxItem = (IMultiChildSyntaxItem) multiChildSyntaxItem.ShallowClone();
                        //Set the current child node of the new syntax item to the simplified syntax item.
                        newMultiChildSyntaxItem.ChildNodes[i] = simplifiedSyntaxItem;
                        newMultiChildSyntaxItem.Compress();
                        //Add the new syntax item to the list of simplifications.
                        simplifications.Add(new SimplificationRulePair(newMultiChildSyntaxItem, reason));
                    }
                    //If a simplification has been found, then no further simplifications should be found.
                    if(simplifications.Count > 0)
                        break;
                }
                break;
            }
        }

        return simplifications;
    }

    /// <summary>
    /// Returns a list of simplifications and their associated reasons that a given syntax tree can be substituted for.
    /// This is based off the simplification rules that are provided.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree that is being simplified.</param>
    /// <param name="simplificationRules">The simplification rules that can be applied to the given syntax tree.</param>
    /// <returns>A list of simplifications and their associated reasons that the given syntax tree can be converted into.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="syntaxTree"/> or <paramref name="simplificationRules"/> is null.</exception>
    private static List<SimplificationRulePair> GetSimplificationsForSyntaxTreeFromSimplificationRules(this ISyntaxItem syntaxTree, SimplificationRule[] simplificationRules) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRules is null) throw new ArgumentNullException(nameof(simplificationRules));
        List<SimplificationRulePair> simplifiedSyntaxItems = new();
        //Iterate over all of the possible simplification rules at the current precedence level.
        foreach (SimplificationRule simplificationRule in simplificationRules) {
            //If the current syntax tree is not able to be simplified by the current simplification rule, continue to the next simplification rule.
            if (!syntaxTree.TryMatchAndSubstitute(simplificationRule, out SimplificationRulePair? syntaxTreeSimplification))
                continue;
            //If the simplification rule was able to be applied, add the new syntax tree and its simplification reason to the list of simplification reasons.
            simplifiedSyntaxItems.Add(syntaxTreeSimplification);
            //If multiple simplification rules are not allowed, break out of the loop.
            if (!simplificationRule.AllowMultiple)
                break;
        }

        return simplifiedSyntaxItems;
    }

    /// <summary>
    /// Returns true if the given syntax tree can be simplified by the given simplification rule.
    /// If this method returns true, then the simplification reason parameter will contain the substituted syntax tree and the reason for the simplification.
    /// If this method returns false, then the simplification reason parameter will be null.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree that is being simplified.</param>
    /// <param name="simplificationRule">The simplification rule that is being applied to the given syntax tree.</param>
    /// <param name="simplificationReason">The substituted form of the syntax tree and the associated reason for the simplification. If it is not substituted correctly, this is null.</param>
    /// <returns>True if the given syntax tree was successfully substituted with the given simplification rule.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the either <paramref name="syntaxTree"/> or <paramref name="simplificationRule"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the right hand side of the simplification rule could not be substituted with the found matches.</exception>
    private static bool TryMatchAndSubstitute(this ISyntaxItem syntaxTree, SimplificationRule simplificationRule, [NotNullWhen(true)] out SimplificationRulePair? simplificationReason) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRule is null) throw new ArgumentNullException(nameof(simplificationRule));
        //Get all of the matches that are possible for the syntax tree and the simplification rule.
        List<Matches> availableMatches = SimplificationRuleMatcher.GetAllMatches(syntaxTree, simplificationRule.LeftHandSide);
        if (availableMatches.Count == 0) {
            //If no matches were found then there is no simplification possible.
            simplificationReason = default;
            return false;
        }

        //Attempt to substitute the simplification rule with the found matches.
        if (!SimplificationRuleSubstituter.TrySubstituteSyntaxTree(simplificationRule.RightHandSide, availableMatches[0], out ISyntaxItem? substitutedSyntaxTree))
            //If this fails, it means that there is a programming error as the right hand side should always be able to be substituted with the found matches.
            throw new InvalidOperationException("The right hand side of the simplification rule was not able to be substituted with the found matches.");
        //If the substitution was successful then return the result with the description of the simplification.
        simplificationReason = new SimplificationRulePair(substitutedSyntaxTree, simplificationRule.Description);
        return true;
    }
}