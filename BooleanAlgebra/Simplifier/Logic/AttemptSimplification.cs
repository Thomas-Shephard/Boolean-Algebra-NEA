namespace BooleanAlgebra.Simplifier.Logic;
public static class AttemptSimplification {
    public static List<Tuple<ISyntaxItem, string>> SimplifySyntaxTree(this ISyntaxItem syntaxTree, SimplificationPost simplificationPost) {
       IEnumerable<IGrouping<int, SimplificationRule>> groupedByPrecedenceSimplificationRules = SimplificationRule.GetSimplificationRules()
           .Where(simplificationRule => simplificationRule.SimplificationPost == simplificationPost)
           .OrderBy(simplificationRule => simplificationRule.Precedence)
           .GroupBy(simplificationRule => simplificationRule.Precedence);

        foreach (IGrouping<int, SimplificationRule> groupedSimplificationRule in groupedByPrecedenceSimplificationRules) {
            IEnumerable<IGrouping<SimplificationPattern, SimplificationRule>> simplificationRules = groupedSimplificationRule
                .Select(x => x)
                .GroupBy(x => x.SimplificationPattern);
            
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
        if (simplificationRules.First().SimplificationPattern is SimplificationPattern.INSIDE_OUT) {
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
            case ISingleDaughterSyntaxItem singleDaughterSyntaxItem: {
                List<Tuple<ISyntaxItem, string>> daughterSimplifications = singleDaughterSyntaxItem.Daughter.SimplifySyntaxTreeWithSimplificationRule(simplificationRules);
                foreach ((ISyntaxItem? item1, string? item2) in daughterSimplifications) {
                    ISyntaxItem simplifiedSyntaxItem = syntaxTree switch {
                        UnaryOperator => new UnaryOperator(syntaxTree.Identifier, item1),
                        _ => throw new Exception()
                    };
                    simplifications.Add(new Tuple<ISyntaxItem, string>(simplifiedSyntaxItem, item2));
                }
                break;
            }
            case IMultipleDaughterSyntaxItem multipleDaughterSyntaxItem: {
                for (int i = 0; i < multipleDaughterSyntaxItem.Daughters.Length; i++) {
                    List<Tuple<ISyntaxItem, string>> daughterSimplifications = multipleDaughterSyntaxItem.Daughters[i].SimplifySyntaxTreeWithSimplificationRule(simplificationRules);
                    foreach ((ISyntaxItem? item1, string? item2) in daughterSimplifications) {
                        ISyntaxItem[] newDaughters = multipleDaughterSyntaxItem.Daughters.ToArray();
                        newDaughters[i] = item1;
                        ISyntaxItem simplifiedSyntaxItem = multipleDaughterSyntaxItem switch {
                            BinaryOperator => new BinaryOperator(syntaxTree.Identifier, newDaughters),
                            _ => throw new Exception()
                        };
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
        foreach (Matches matches in SyntaxTreeMatch.GetAllMatches(syntaxTree, simplificationRule.LeftHandSide)) {
            if (!SyntaxTreeSubstitution.TrySubstituteSyntaxTree(simplificationRule.RightHandSide, matches, out ISyntaxItem? substitutedSyntaxTree))
                continue;
            syntaxTreeSimplification = new Tuple<ISyntaxItem, string>(substitutedSyntaxTree, simplificationRule.Message);
            return true;
        }

        syntaxTreeSimplification = default;
        return false;
    }
}