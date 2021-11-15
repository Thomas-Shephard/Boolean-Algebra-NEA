namespace BooleanAlgebra.Simplification;

public class SimplificationRule {
    public SyntaxItem LeftHandSide { get; }
    public SyntaxItem RightHandSide { get; }
    public string Message { get; }
    public int Precedence { get; }
    public SimplificationPattern SimplificationPattern { get; }
    public SimplificationPost SimplificationPost { get; }

    private SimplificationRule(string leftHandSide, string rightHandSide, string message, int precedence, SimplificationPattern simplificationPattern, SimplificationPost simplificationPost) {
        //Lexing the left and right hand sides
        if (!new Lexer.Lexer(leftHandSide).Lex(out List<Lexeme> leftHandSideLexemes))
            throw new ArgumentException(nameof(leftHandSide));
        if (!new Lexer.Lexer(rightHandSide).Lex(out List<Lexeme> rightHandSideLexemes))
            throw new ArgumentException(nameof(rightHandSide));
        //Parsing the left and right hand sides
        if (!new Parser.Parser(leftHandSideLexemes, true).TryParse(out SyntaxItem? leftHandSideSyntaxTree))
            throw new ArgumentException(nameof(leftHandSideLexemes));
        if (!new Parser.Parser(rightHandSideLexemes, true).TryParse(out SyntaxItem? rightHandSideSyntaxTree))
            throw new ArgumentException(nameof(rightHandSideLexemes));

        LeftHandSide = leftHandSideSyntaxTree;
        RightHandSide = rightHandSideSyntaxTree;
        Message = message;
        Precedence = precedence;
        SimplificationPattern = simplificationPattern;
        SimplificationPost = simplificationPost;
    }

    private static SimplificationRule[][]? _simplificationRules;

    public static IEnumerable<SimplificationRule[]> GetSimplificationRules() {
        return _simplificationRules ??= GenerateSimplificationRules();
    }

    private static SimplificationRule[][] GenerateSimplificationRules() {
        return new [] {
            new[] {
                //AND Laws
                new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND !ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement Law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //OR Laws
                new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR !ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement Law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //NOT Laws
                new SimplificationRule("!0", "1", "NOT 0 is 1", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!1", "0", "NOT 1 is 0", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!!ItemA", "ItemA", "Involution Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //Absorption Laws
                new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "!ItemA OR (ItemB AND [ItemsA]) OR [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA AND (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "!ItemA AND (ItemB OR [ItemsA]) AND [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //De Morgan's Laws
                new SimplificationRule("!(ItemA AND ItemB AND [ItemsA])", "!ItemA OR !ItemB OR [!ItemsA]", "De Morgan's Law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!(ItemA OR ItemB OR [ItemsA])", "!ItemA AND !ItemB AND [!ItemsA]", "De Morgan's Law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR !ItemB OR [!ItemsA] OR [ItemsB]", "!(ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                new SimplificationRule("!ItemA AND !ItemB AND [!ItemsA] AND [ItemsB]", "!(ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                //Distributive Laws
                new SimplificationRule("(ItemA AND ItemB AND [ItemsB]) OR (ItemA AND ItemC AND [ItemsC]) OR [ItemsA]", "[ItemsA] OR (ItemA AND ((ItemB AND [ItemsB]) OR (ItemC AND [ItemsC])))", "Distributive Law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("(ItemA OR ItemB OR [ItemsB]) AND (ItemA OR ItemC OR [ItemsC]) AND [ItemsA]", "[ItemsA] AND (ItemA OR ((ItemB OR [ItemsB]) AND (ItemC OR [ItemsC])))", "Distributive Law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive Law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive Law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
            },
            new[] {
                //AND Laws
                new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND !ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement Law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //OR Laws
                new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR !ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement Law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity Law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //NOT Laws
                new SimplificationRule("!0", "1", "NOT 0 is 1", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!1", "0", "NOT 1 is 0", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!!ItemA", "ItemA", "Involution Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //Absorption Laws
                new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "!ItemA OR (ItemB AND [ItemsA]) OR [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA AND (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "!ItemA AND (ItemB OR [ItemsA]) AND [ItemsB]", "Absorption Law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //De Morgan's Laws
                new SimplificationRule("!(ItemA AND ItemB AND [ItemsA])", "!ItemA OR !ItemB OR [!ItemsA]", "De Morgan's Law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!(ItemA OR ItemB OR [ItemsA])", "!ItemA AND !ItemB AND [!ItemsA]", "De Morgan's Law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR !ItemB OR [!ItemsA] OR [ItemsB]", "!(ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                new SimplificationRule("!ItemA AND !ItemB AND [!ItemsA] AND [ItemsB]", "!(ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                //Distributive Laws
                new SimplificationRule("(ItemA AND ItemB AND [ItemsB]) OR (ItemA AND ItemC AND [ItemsC]) OR [ItemsA]", "[ItemsA] OR (ItemA AND ((ItemB AND [ItemsB]) OR (ItemC AND [ItemsC])))", "Distributive Law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("(ItemA OR ItemB OR [ItemsB]) AND (ItemA OR ItemC OR [ItemsC]) AND [ItemsA]", "[ItemsA] AND (ItemA OR ((ItemB OR [ItemsB]) AND (ItemC OR [ItemsC])))", "Distributive Law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive Law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive Law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
            },
        };
    }
}