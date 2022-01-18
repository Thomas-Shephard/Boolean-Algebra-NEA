namespace BooleanAlgebra.Simplifier.Logic;
public class SimplificationRule {
    public ISyntaxItem LeftHandSide { get; }
    public ISyntaxItem RightHandSide { get; }
    public string Message { get; }
    public int Precedence { get; }
    public SimplificationPattern SimplificationPattern { get; }
    public SimplificationOrder SimplificationOrder { get; }
    public bool AllowMultiple { get; }

    private SimplificationRule(string leftHandSide, string rightHandSide, string message, int precedence, SimplificationPattern simplificationPattern, SimplificationOrder simplificationOrder, bool allowMultiple) {
        List<Lexeme> leftHandSideLexemes = new Lexer.Lexer(leftHandSide, true).Lex();
        List<Lexeme> rightHandSideLexemes = new Lexer.Lexer(rightHandSide, true).Lex();
        
        LeftHandSide = new Parser.Parser(leftHandSideLexemes, true).Parse();
        RightHandSide = new Parser.Parser(rightHandSideLexemes, true).Parse();
        Message = message;
        Precedence = precedence;
        SimplificationPattern = simplificationPattern;
        SimplificationOrder = simplificationOrder;
        AllowMultiple = allowMultiple;
    }

    private static SimplificationRule[]? _simplificationRules;

    public static IEnumerable<SimplificationRule> GetSimplificationRules() {
        return _simplificationRules ??= GenerateSimplificationRules();
    }

    private static SimplificationRule[] GenerateSimplificationRules() {
        return new[] {
            //AND Laws
            new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA AND !ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //OR Laws
            new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR !ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //NOT Laws
            new SimplificationRule("!0", "1", "Negation law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!1", "0", "Negation law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!!ItemA", "ItemA", "Involution law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //Absorption Laws
            new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!ItemA OR (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "!ItemA OR (ItemB AND [ItemsA]) OR [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!ItemA AND (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "!ItemA AND (ItemB OR [ItemsA]) AND [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //De Morgan's Laws
            new SimplificationRule("!(ItemA AND ItemB AND [ItemsA])", "!ItemA OR !ItemB OR [!ItemsA]", "De Morgan's law", 4, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!(ItemA OR ItemB OR [ItemsA])", "!ItemA AND !ItemB AND [!ItemsA]", "De Morgan's law", 4, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!ItemA OR !ItemB OR [!ItemsA] OR [ItemsB]", "!(ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.POST, false),
            new SimplificationRule("!ItemA AND !ItemB AND [!ItemsA] AND [ItemsB]", "!(ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationOrder.POST, false),
            //Distributive Laws
            new SimplificationRule("(ItemA AND ItemB AND [ItemsB]) OR (ItemA AND ItemC AND [ItemsC]) OR [ItemsA]", "[ItemsA] OR (ItemA AND ((ItemB AND [ItemsB]) OR (ItemC AND [ItemsC])))", "Distributive law", 5, SimplificationPattern.INSIDE_OUT, SimplificationOrder.PRE, true),
            new SimplificationRule("(ItemA OR ItemB OR [ItemsB]) AND (ItemA OR ItemC OR [ItemsC]) AND [ItemsA]", "[ItemsA] AND (ItemA OR ((ItemB OR [ItemsB]) AND (ItemC OR [ItemsC])))", "Distributive law", 5, SimplificationPattern.INSIDE_OUT, SimplificationOrder.PRE, true),
            new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive law", 5, SimplificationPattern.INSIDE_OUT, SimplificationOrder.PRE, true),
            new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive law", 5, SimplificationPattern.INSIDE_OUT, SimplificationOrder.PRE, true),
        };
    }
}