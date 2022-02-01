namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// 
/// </summary>
public static class AttemptSimplification {
    public static IEnumerable<SimplificationReason> SimplifySyntaxTree(this ISyntaxItem syntaxTree, bool isPostSimplification) {
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
            List<SimplificationReason> simplifications = syntaxTree.SimplifySyntaxTreeWithSimplificationRules(groupedSimplificationRulesByPrecedence.ToArray());
            //If any simplification was made, return the list of simplifications.
            if (simplifications.Count > 0)
                return simplifications;
        }

        //If no simplification was made, return an empty list.
        return new List<SimplificationReason>();
    }

    private static List<SimplificationReason> SimplifySyntaxTreeWithSimplificationRules(this ISyntaxItem syntaxTree, SimplificationRule[] simplificationRules) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRules is null) throw new ArgumentNullException(nameof(simplificationRules));
        //All the simplifications at the same precedence should have the same simplification traversal order.
        //Therefore, getting the traversal order from the first rule in the list is sufficient.
        if (simplificationRules[0].SimplificationTraversalOrder is SimplificationTraversalOrder.INSIDE_OUT) {
            //If the traversal order is INSIDE_OUT, then the simplification rules should be applied to the child nodes of the current syntax item.
            List<SimplificationReason> simplifications = syntaxTree.SimplifySyntaxTreeChildNodesWithSimplificationRules(simplificationRules);
            //If any simplifications are possible, return the list of simplifications.
            //Otherwise, attempt to simplify the current syntax item with the simplification rules.
            return simplifications.Count > 0 
                ? simplifications 
                : syntaxTree.SimplifySyntaxTrees(simplificationRules);
        } else {
            //If the traversal order is not INSIDE_OUT, it must be OUTSIDE_IN.
            //In this case, the simplification rules should be applied to the current syntax item.
            List<SimplificationReason> simplifications = syntaxTree.SimplifySyntaxTrees(simplificationRules);
            //If any simplifications are possible, return the list of simplifications.
            //Otherwise, attempt to simplify the child nodes of the current syntax item with the simplification rules.
            return simplifications.Count > 0 
                ? simplifications 
                : syntaxTree.SimplifySyntaxTreeChildNodesWithSimplificationRules(simplificationRules);
        }
    }

    private static List<SimplificationReason> SimplifySyntaxTreeChildNodesWithSimplificationRules(this ISyntaxItem syntaxTree, SimplificationRule[] simplificationRules) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRules is null) throw new ArgumentNullException(nameof(simplificationRules));
        List<SimplificationReason> simplifications = new();
        switch (syntaxTree) {
            case ISingleChildSyntaxItem singleChildSyntaxItem: {
                List<SimplificationReason> childNodeSimplifications = singleChildSyntaxItem.ChildNode.SimplifySyntaxTreeWithSimplificationRules(simplificationRules);
                foreach ((ISyntaxItem simplifiedSyntaxItem, string reason) in childNodeSimplifications) {
                    ISingleChildSyntaxItem newSingleChildSyntaxItem = (ISingleChildSyntaxItem) singleChildSyntaxItem.ShallowClone();
                    newSingleChildSyntaxItem.ChildNode = simplifiedSyntaxItem;
                    simplifications.Add(new SimplificationReason(newSingleChildSyntaxItem, reason));
                }
                break;
            }
            case IMultiChildSyntaxItem multiChildSyntaxItem: {
                for (int i = 0; i < multiChildSyntaxItem.ChildNodes.Length; i++) {
                    List<SimplificationReason> childNodeSimplifications = multiChildSyntaxItem.ChildNodes[i].SimplifySyntaxTreeWithSimplificationRules(simplificationRules);
                    foreach ((ISyntaxItem simplifiedSyntaxItem, string reason) in childNodeSimplifications) {
                        IMultiChildSyntaxItem newMultiChildSyntaxItem = (IMultiChildSyntaxItem) multiChildSyntaxItem.ShallowClone();
                        newMultiChildSyntaxItem.ChildNodes[i] = simplifiedSyntaxItem;
                        newMultiChildSyntaxItem.Compress();
                        simplifications.Add(new SimplificationReason(newMultiChildSyntaxItem, reason));
                    }
                    if(simplifications.Count > 0)
                        break;
                }
                break;
            }
        }

        return simplifications;
    }

    private static List<SimplificationReason> SimplifySyntaxTrees(this ISyntaxItem syntaxTree, SimplificationRule[] simplificationRules) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRules is null) throw new ArgumentNullException(nameof(simplificationRules));
        List<SimplificationReason> simplifiedSyntaxItems = new();
        
        foreach (SimplificationRule simplificationRule in simplificationRules) {
            if (syntaxTree.TryMatchAndSubstitute(simplificationRule, out SimplificationReason? syntaxTreeSimplification))
                simplifiedSyntaxItems.Add(syntaxTreeSimplification);
            if (!simplificationRule.AllowMultiple && simplifiedSyntaxItems.Count > 0)
                break;
        }

        return simplifiedSyntaxItems;
    }

    private static bool TryMatchAndSubstitute(this ISyntaxItem syntaxTree, SimplificationRule simplificationRule, [NotNullWhen(true)] out SimplificationReason? simplificationReason) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRule is null) throw new ArgumentNullException(nameof(simplificationRule));
        //Get all of the matches that are possible for the syntax tree and the simplification rule.
        List<Matches> availableMatches = SyntaxTreeMatch.GetAllMatches(syntaxTree, simplificationRule.LeftHandSide);
        if (availableMatches.Count == 0) {
            //If no matches were found then there is no simplification possible.
            simplificationReason = default;
            return false;
        }

        //Attempt to substitute the simplification rule with the found matches.
        if (!SyntaxTreeSubstitution.TrySubstituteSyntaxTree(simplificationRule.RightHandSide, availableMatches[0], out ISyntaxItem? substitutedSyntaxTree))
            //If this fails, it means that there is a programming error as the right hand side should always be able to be substituted with the found matches.
            throw new InvalidOperationException("The right hand side of the simplification rule was not able to be substituted with the found matches.");
        //If the substitution was successful then return the result with the description of the simplification.
        simplificationReason = new SimplificationReason(substitutedSyntaxTree, simplificationRule.Description);
        return true;
    }
}