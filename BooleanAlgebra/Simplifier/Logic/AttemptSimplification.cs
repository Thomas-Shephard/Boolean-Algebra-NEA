using BooleanAlgebra.Simplification;

namespace BooleanAlgebra.Simplifier.Logic;

public static class AttemptSimplification {
    public static bool TrySimplifySyntaxTree(this SyntaxItem syntaxTree, int index, IReadOnlyCollection<SyntaxItem> previousSimplifications, SimplificationPost simplificationPost, [NotNullWhen(true)] out Tuple<SyntaxItem, string>? syntaxTreeSimplification) {
       IEnumerable<IGrouping<int, SimplificationRule>> groupedByPrecedenceSimplificationRules = SimplificationRule.GetSimplificationRules().ElementAt(index)
           .Where(simplificationRule => simplificationRule.SimplificationPost == simplificationPost)
           .OrderBy(simplificationRule => simplificationRule.Precedence)
           .GroupBy(simplificationRule => simplificationRule.Precedence);

        foreach (IGrouping<int, SimplificationRule> groupedSimplificationRule in groupedByPrecedenceSimplificationRules) {
            IEnumerable<IGrouping<SimplificationPattern, SimplificationRule>> simplificationRules = groupedSimplificationRule
                .Select(x => x)
                .GroupBy(x => x.SimplificationPattern);
            
            foreach (IEnumerable<SimplificationRule> simplificationRule in simplificationRules.Select(x=> x)) {
                if(syntaxTree.TrySimplifySyntaxTreeWithSimplificationRule(simplificationRule.ToList(), previousSimplifications, out syntaxTreeSimplification))
                    return true;
            }
        }

        syntaxTreeSimplification = default;
        return false;
    }

    private static bool TrySimplifySyntaxTreeWithSimplificationRule(this SyntaxItem syntaxTree, List<SimplificationRule> simplificationRules, IReadOnlyCollection<SyntaxItem> previousSimplifications, [NotNullWhen(true)] out Tuple<SyntaxItem, string>? syntaxTreeSimplification) {
        simplificationRules = simplificationRules.ToList();
        if (simplificationRules.First().SimplificationPattern is SimplificationPattern.INSIDE_OUT) {
            if (syntaxTree.TrySimplifySyntaxTreeDaughterItemsWithSimplificationRule(simplificationRules, previousSimplifications, out syntaxTreeSimplification))
                return true;
            if (syntaxTree.TrySimplifySyntaxTree(simplificationRules, previousSimplifications, out syntaxTreeSimplification))
                return true;
        } else {
            if (syntaxTree.TrySimplifySyntaxTree(simplificationRules, previousSimplifications, out syntaxTreeSimplification))
                return true;
            if (syntaxTree.TrySimplifySyntaxTreeDaughterItemsWithSimplificationRule(simplificationRules, previousSimplifications, out syntaxTreeSimplification))
                return true;
        }
        
        syntaxTreeSimplification = default;
        return false;
    }

    private static bool TrySimplifySyntaxTreeDaughterItemsWithSimplificationRule(this SyntaxItem syntaxTree, List<SimplificationRule> simplificationRules, IReadOnlyCollection<SyntaxItem> previousSimplifications, [NotNullWhen(true)] out Tuple<SyntaxItem, string>? syntaxTreeSimplification) {
        for (int i = 0; i < syntaxTree.DaughterItems.Count; i++) {
            if(!syntaxTree.DaughterItems[i].TrySimplifySyntaxTreeWithSimplificationRule(simplificationRules, previousSimplifications, out Tuple<SyntaxItem, string>? daughterSyntaxTreeSimplification))
                continue;
            SyntaxItem simplifiedSyntaxItem = syntaxTree.Clone();
            simplifiedSyntaxItem.DaughterItems[i] = daughterSyntaxTreeSimplification.Item1;
            simplifiedSyntaxItem = simplifiedSyntaxItem.Compress();
            
            syntaxTreeSimplification = new Tuple<SyntaxItem, string>(simplifiedSyntaxItem, daughterSyntaxTreeSimplification.Item2);
            return true;
        }

        syntaxTreeSimplification = default;
        return false;
    }

    private static bool TrySimplifySyntaxTree(this SyntaxItem syntaxTree, List<SimplificationRule> simplificationRules, IReadOnlyCollection<SyntaxItem> previousSimplifications, [NotNullWhen(true)] out Tuple<SyntaxItem, string>? syntaxTreeSimplification) {
        foreach (SimplificationRule simplificationRule in simplificationRules) {
            if (syntaxTree.TryMatchAndSubstitute(simplificationRule, previousSimplifications, out syntaxTreeSimplification))
                return true;
        }

        syntaxTreeSimplification = default;
        return false;
    }

    private static bool TryMatchAndSubstitute(this SyntaxItem syntaxTree, SimplificationRule simplificationRule, IReadOnlyCollection<SyntaxItem> previousSimplifications, [NotNullWhen(true)] out Tuple<SyntaxItem, string>? syntaxTreeSimplification) {
        foreach (Matches matches in SyntaxTreeMatch.GetAllMatches(syntaxTree, simplificationRule.LeftHandSide)) {
            if (!SyntaxTreeSubstitution.TrySubstituteSyntaxTree(simplificationRule.RightHandSide, matches, out SyntaxItem? substitutedSyntaxTree))
                continue;
            if(previousSimplifications.Any(x => x.Equals(substitutedSyntaxTree)))
                continue;
            syntaxTreeSimplification = new Tuple<SyntaxItem, string>(substitutedSyntaxTree, simplificationRule.Message);
            return true;
        }

        syntaxTreeSimplification = default;
        return false;
    }
}