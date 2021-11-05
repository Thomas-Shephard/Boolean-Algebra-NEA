namespace BooleanAlgebra.Simplification.Logic; 

public static class AttemptSimplification {
    public static bool TrySimplify(SyntaxItem syntaxTree, [NotNullWhen(true)] out Tuple<SyntaxItem, string>? syntaxTreeSimplification) {
        foreach (SimplificationRule simplificationRule in SimplificationRule.GetSimplificationRules()) {
            if (syntaxTree.TryMatchAndSubstitute(simplificationRule, out syntaxTreeSimplification)) {
                return true;
            }
        }

        SyntaxItem simplifiedSyntaxItem = syntaxTree.Clone();
        for (int i = 0; i < simplifiedSyntaxItem.DaughterItems.Count; i++) {
            if (!TrySimplify(simplifiedSyntaxItem.DaughterItems[i], out Tuple<SyntaxItem, string>? simplifiedDaughterItem)) continue;
            simplifiedSyntaxItem.DaughterItems[i] = simplifiedDaughterItem.Item1;
            syntaxTreeSimplification = new Tuple<SyntaxItem, string>(simplifiedSyntaxItem.Clone(), simplifiedDaughterItem.Item2);
            return true;
        }

        syntaxTreeSimplification = default;
        return false;
    }

    private static bool TryMatchAndSubstitute(this SyntaxItem syntaxTree, SimplificationRule simplificationRule, [NotNullWhen(true)] out Tuple<SyntaxItem, string>? syntaxTreeSimplification) {
        Dictionary<string, List<SyntaxItem>> substituteItems = new();
        Dictionary<string, SyntaxItem> directSubstitutes = new();
        if (!SyntaxTreeMatch.IsMatch(syntaxTree, simplificationRule.LeftHandSide, ref substituteItems, ref directSubstitutes)) {
            syntaxTreeSimplification = default;
            return false;
        }

        if (!SyntaxTreeSubstitution.TrySubstituteSyntaxTree(simplificationRule.RightHandSide, substituteItems, directSubstitutes, out SyntaxItem? substitutedSyntaxTree)) {
            syntaxTreeSimplification = default;
            return false;
        }

        syntaxTreeSimplification = new Tuple<SyntaxItem, string>(substitutedSyntaxTree, simplificationRule.Message);
        return true;
    }
}