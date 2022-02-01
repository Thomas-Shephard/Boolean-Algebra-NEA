namespace BooleanAlgebra.Simplifier.Logic;
public static class AttemptSimplification {
    public static IEnumerable<Tuple<ISyntaxItem, string>> SimplifySyntaxTree(this ISyntaxItem syntaxTree, bool isPostSimplification) {
       IEnumerable<IGrouping<int, SimplificationRule>> groupedByPrecedenceSimplificationRules = SimplificationRule.GetSimplificationRules()
           .Where(simplificationRule => simplificationRule.IsPostSimplification == isPostSimplification)
           .OrderBy(simplificationRule => simplificationRule.Precedence)
           .GroupBy(simplificationRule => simplificationRule.Precedence);

        foreach (IGrouping<int, SimplificationRule> groupedSimplificationRule in groupedByPrecedenceSimplificationRules) {
            IEnumerable<IGrouping<SimplificationTraversalOrder, SimplificationRule>> simplificationRules = groupedSimplificationRule
                .Select(x => x)
                .GroupBy(x => x.SimplificationTraversalOrder);
            
            foreach (IEnumerable<SimplificationRule> simplificationRule in simplificationRules.Select(x=> x)) {
                List<Tuple<ISyntaxItem, string>> simplifications = syntaxTree.SimplifySyntaxTreeWithSimplificationRule(simplificationRule.ToList());
                if (simplifications.Count > 0)
                    return simplifications;
            }
        }

        return new List<Tuple<ISyntaxItem, string>>();
    }

    private static List<Tuple<ISyntaxItem, string>> SimplifySyntaxTreeWithSimplificationRule(this ISyntaxItem syntaxTree, List<SimplificationRule> simplificationRules) {
        simplificationRules = simplificationRules.ToList();
        if (simplificationRules.First().SimplificationTraversalOrder is SimplificationTraversalOrder.INSIDE_OUT) {
            List<Tuple<ISyntaxItem, string>> simplifications = syntaxTree.TrySimplifySyntaxTreeDaughterItemsWithSimplificationRule(simplificationRules);
            if (simplifications.Count > 0)
                return simplifications;
            return syntaxTree.SimplifySyntaxTrees(simplificationRules);
        } else {
            List<Tuple<ISyntaxItem, string>> simplifications = syntaxTree.SimplifySyntaxTrees(simplificationRules);
            if (simplifications.Count > 0)
                return simplifications;
            return syntaxTree.TrySimplifySyntaxTreeDaughterItemsWithSimplificationRule(simplificationRules);
        }
    }

    private static List<Tuple<ISyntaxItem, string>> TrySimplifySyntaxTreeDaughterItemsWithSimplificationRule(this ISyntaxItem syntaxTree, List<SimplificationRule> simplificationRules) {
        List<Tuple<ISyntaxItem, string>> simplifications = new();
        switch (syntaxTree) {
            case ISingleChildSyntaxItem singleDaughterSyntaxItem: {
                List<Tuple<ISyntaxItem, string>> daughterSimplifications = singleDaughterSyntaxItem.ChildNode.SimplifySyntaxTreeWithSimplificationRule(simplificationRules);
                foreach ((ISyntaxItem? item1, string? item2) in daughterSimplifications) {
                    ISyntaxItem simplifiedSyntaxItem = syntaxTree switch {
                        UnaryOperator => new UnaryOperator(syntaxTree.Identifier, item1),
                        _ => throw new Exception()
                    };
                    simplifications.Add(new Tuple<ISyntaxItem, string>(simplifiedSyntaxItem, item2));
                }
                break;
            }
            case IMultiChildSyntaxItem multiChildSyntaxItem: {
                for (int i = 0; i < multiChildSyntaxItem.ChildNodes.Length; i++) {
                    List<Tuple<ISyntaxItem, string>> daughterSimplifications = multiChildSyntaxItem.ChildNodes[i].SimplifySyntaxTreeWithSimplificationRule(simplificationRules);
                    foreach ((ISyntaxItem? item1, string? item2) in daughterSimplifications) {
                        ISyntaxItem[] newDaughters = multiChildSyntaxItem.ChildNodes.ToArray();
                        newDaughters[i] = item1;
                        ISyntaxItem simplifiedSyntaxItem = multiChildSyntaxItem switch {
                            BinaryOperator => new BinaryOperator(syntaxTree.Identifier, newDaughters),
                            _ => throw new Exception()
                        };
                        simplifiedSyntaxItem.Compress();
                        simplifications.Add(new Tuple<ISyntaxItem, string>(simplifiedSyntaxItem, item2));
                    }
                    if(simplifications.Count > 0)
                        break;
                }
                break;
            }
        }

        return simplifications;
    }

    private static List<Tuple<ISyntaxItem, string>> SimplifySyntaxTrees(this ISyntaxItem syntaxTree, List<SimplificationRule> simplificationRules) {
        List<Tuple<ISyntaxItem, string>> simplifiedSyntaxItems = new();
        
        foreach (SimplificationRule simplificationRule in simplificationRules) {
            if (syntaxTree.TryMatchAndSubstitute(simplificationRule, out Tuple<ISyntaxItem, string>? syntaxTreeSimplification))
                simplifiedSyntaxItems.Add(syntaxTreeSimplification);
            if (!simplificationRule.AllowMultiple && simplifiedSyntaxItems.Count > 0)
                break;
        }

        return simplifiedSyntaxItems;
    }

    private static bool TryMatchAndSubstitute(this ISyntaxItem syntaxTree, SimplificationRule simplificationRule, [NotNullWhen(true)] out Tuple<ISyntaxItem, string>? syntaxTreeSimplification) {
        if (syntaxTree is null) throw new ArgumentNullException(nameof(syntaxTree));
        if (simplificationRule is null) throw new ArgumentNullException(nameof(simplificationRule));
        //Get all of the matches that are possible for the syntax tree and the simplification rule.
        List<Matches> availableMatches = SyntaxTreeMatch.GetAllMatches(syntaxTree, simplificationRule.LeftHandSide);
        if (availableMatches.Count == 0) {
            //If no matches were found then there is no simplification possible.
            syntaxTreeSimplification = default;
            return false;
        }

        //Attempt to substitute the simplification rule with the found matches.
        if (!SyntaxTreeSubstitution.TrySubstituteSyntaxTree(simplificationRule.RightHandSide, availableMatches[0], out ISyntaxItem? substitutedSyntaxTree))
            //If this fails, it means that there is a programming error as the right hand side should always be able to be substituted with the found matches.
            throw new InvalidOperationException("The right hand side of the simplification rule was not able to be substituted with the found matches.");
        //If the substitution was successful then return the result with the description of the simplification.
        syntaxTreeSimplification = new Tuple<ISyntaxItem, string>(substitutedSyntaxTree, simplificationRule.Description);
        return true;
    }
}