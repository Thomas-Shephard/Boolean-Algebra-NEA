namespace BooleanAlgebra.Simplification;
public class SimplificationRule {
    public SyntaxItem LeftHandSide { get; }
    public SyntaxItem RightHandSide { get; }
    public bool IsBiDirectional { get; }
    public string Message { get; }

    public SimplificationRule(string leftHandSide, string rightHandSide, bool isBiDirectional, string message) {
        if (!new Lexer.Lexer(leftHandSide).Lex(out List<Lexeme> leftHandSideLexemes))
            throw new ArgumentException(nameof(leftHandSide));
        if (!new Lexer.Lexer(rightHandSide).Lex(out List<Lexeme> rightHandSideLexemes))
            throw new ArgumentException(nameof(rightHandSide));
        if (!new Parser.Parser(leftHandSideLexemes, true).TryParse(out SyntaxItem? leftHandSideSyntaxTree))
            throw new ArgumentException(nameof(leftHandSideLexemes));
        if (!new Parser.Parser(rightHandSideLexemes, true).TryParse(out SyntaxItem? rightHandSideSyntaxTree))
            throw new ArgumentException(nameof(rightHandSideLexemes));
        LeftHandSide = leftHandSideSyntaxTree;
        RightHandSide = rightHandSideSyntaxTree;
        IsBiDirectional = isBiDirectional;
        Message = message;
    }

    private static IEnumerable<SimplificationRule>? _simplificationRules;

    public static IEnumerable<SimplificationRule> GetSimplificationRules() {
        _simplificationRules ??= GenerateSimplificationRules().ToArray();
        return _simplificationRules;
    }

    private static IEnumerable<SimplificationRule> GenerateSimplificationRules() {
        return new[] {
            //AND
            new SimplificationRule("A AND A", "A", false, "Idempotent Law"),
            new SimplificationRule("A AND NOT A", "0", false, "Complement Law"),
            new SimplificationRule("A AND 0", "0", false, "Annulment Law"),
            new SimplificationRule("A AND 1", "A", false, "Identity Law"),
            //OR
            new SimplificationRule("A OR A", "A", false, "Idempotent Law"),
            new SimplificationRule("A OR NOT A", "1", false, "Complement Law"),
            new SimplificationRule("A OR 0", "A", false, "Identity Law"),
            new SimplificationRule("A OR 1", "1", false, "Annulment Law"),
            //NOT
            new SimplificationRule("NOT 0", "1",  false, "NOT 0 is 1"),
            new SimplificationRule("NOT 1", "0", false, "NOT 1 is 0"),
            new SimplificationRule("NOT NOT A", "A", false, "Involution Law"),
            //De Morgan's Laws
            new SimplificationRule("NOT (A OR B)", "(NOT A) AND (NOT B)", true, "De Morgan's Law"),
            new SimplificationRule("NOT (A AND B)", "(NOT A) OR (NOT B)", true, "De Morgan's Law"),
            //Absorption Laws
            new SimplificationRule("A AND (A OR B)", "A", false, "Absorption Law"),
            new SimplificationRule("A OR (A AND B)", "A", false, "Absorption Law"),
            //Distributive Laws
            new SimplificationRule("A AND (B OR C)", "(A AND B) OR (A AND C)", false, "Distributive Law"),
            new SimplificationRule("A OR (B AND C)", "(A OR B) AND (A OR C)", false, "Distributive Law"),
        };
    }
}