using BooleanAlgebra.Lexer;
using BooleanAlgebra.Simplifier;

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
        bool leftHandSideResult = new Lexer.Lexer(leftHandSide).TryLex(out List<Lexeme> leftHandSideLexemes, out LexerException? leftHandSideLexerException);
        bool rightHandSideResult = new Lexer.Lexer(rightHandSide).TryLex(out List<Lexeme> rightHandSideLexemes, out LexerException? rightHandSideLexerException);
        
        if (!leftHandSideResult)
            throw new ArgumentException(nameof(leftHandSide));
        if (!rightHandSideResult)
            throw new ArgumentException(nameof(rightHandSide));
        //Parsing the left and right hand sides




        Parser.Parser leftHandSideParser = new(leftHandSideLexemes, true);
        Parser.Parser rightHandSideParser = new(rightHandSideLexemes, true);

        SyntaxItem leftHandSideSyntaxTree;
        SyntaxItem rightHandSideSyntaxTree;

            leftHandSideSyntaxTree = leftHandSideParser.Parse();
        rightHandSideSyntaxTree = rightHandSideParser.Parse();


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
                new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND !ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //OR Laws
                new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR !ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //NOT Laws
                new SimplificationRule("!0", "1", "!0 is 1", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!1", "0", "!1 is 0", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!!ItemA", "ItemA", "Involution law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //Absorption Laws
                new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "!ItemA OR (ItemB AND [ItemsA]) OR [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA AND (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "!ItemA AND (ItemB OR [ItemsA]) AND [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //De Morgan's Laws
                new SimplificationRule("!(ItemA AND ItemB AND [ItemsA])", "!ItemA OR !ItemB OR [!ItemsA]", "De Morgan's law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!(ItemA OR ItemB OR [ItemsA])", "!ItemA AND !ItemB AND [!ItemsA]", "De Morgan's law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR !ItemB OR [!ItemsA] OR [ItemsB]", "!(ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                new SimplificationRule("!ItemA AND !ItemB AND [!ItemsA] AND [ItemsB]", "!(ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                //Distributive Laws
                new SimplificationRule("(ItemA AND ItemB AND [ItemsB]) OR (ItemA AND ItemC AND [ItemsC]) OR [ItemsA]", "[ItemsA] OR (ItemA AND ((ItemB AND [ItemsB]) OR (ItemC AND [ItemsC])))", "Distributive law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("(ItemA OR ItemB OR [ItemsB]) AND (ItemA OR ItemC OR [ItemsC]) AND [ItemsA]", "[ItemsA] AND (ItemA OR ((ItemB OR [ItemsB]) AND (ItemC OR [ItemsC])))", "Distributive law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
            },
            new[] {
                //AND Laws
                new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND !ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //OR Laws
                new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR !ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement law", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity law", 3, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment Law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //NOT Laws
                new SimplificationRule("!0", "1", "!0 is 1", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!1", "0", "!1 is 0", 1, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!!ItemA", "ItemA", "Involution law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //Absorption Laws
                new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "!ItemA OR (ItemB AND [ItemsA]) OR [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA AND (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "!ItemA AND (ItemB OR [ItemsA]) AND [ItemsB]", "Absorption law", 2, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                //De Morgan's Laws
                new SimplificationRule("!(ItemA AND ItemB AND [ItemsA])", "!ItemA OR !ItemB OR [!ItemsA]", "De Morgan's law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!(ItemA OR ItemB OR [ItemsA])", "!ItemA AND !ItemB AND [!ItemsA]", "De Morgan's law", 5, SimplificationPattern.OUTSIDE_IN, SimplificationPost.BEFORE),
                new SimplificationRule("!ItemA OR !ItemB OR [!ItemsA] OR [ItemsB]", "!(ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                new SimplificationRule("!ItemA AND !ItemB AND [!ItemsA] AND [ItemsB]", "!(ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's law", 0, SimplificationPattern.OUTSIDE_IN, SimplificationPost.AFTER),
                //Distributive Laws
                new SimplificationRule("(ItemA AND ItemB AND [ItemsB]) OR (ItemA AND ItemC AND [ItemsC]) OR [ItemsA]", "[ItemsA] OR (ItemA AND ((ItemB AND [ItemsB]) OR (ItemC AND [ItemsC])))", "Distributive law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("(ItemA OR ItemB OR [ItemsB]) AND (ItemA OR ItemC OR [ItemsC]) AND [ItemsA]", "[ItemsA] AND (ItemA OR ((ItemB OR [ItemsB]) AND (ItemC OR [ItemsC])))", "Distributive law", 6, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
                new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive law", 4, SimplificationPattern.INSIDE_OUT, SimplificationPost.BEFORE),
            },
        };
    }
}