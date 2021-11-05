using System.Diagnostics;

namespace BooleanAlgebra.Simplification;

public class SimplificationRule {
    public SyntaxItem LeftHandSide { get; }
    public SyntaxItem RightHandSide { get; }
    public string Message { get; }

    
    private SimplificationRule(string leftHandSide, string rightHandSide, string message) {
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
    }

    private static IEnumerable<SimplificationRule>? _simplificationRules;

    public static IEnumerable<SimplificationRule> GetSimplificationRules() {
        _simplificationRules ??= Debugger.IsAttached 
            ? GenerateDebugSimplificationRules().ToArray()
            : GenerateSimplificationRules().ToArray();

        return _simplificationRules;
    }
    
    private static IEnumerable<SimplificationRule> GenerateDebugSimplificationRules() {
        return new[] {
            //AND Laws (4/4)
            new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent Law"),
            new SimplificationRule("ItemA AND NOT ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement Law"),
            new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment Law"),
            new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity Law"),
            //OR Laws (4/4)
            new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent Law"),
            new SimplificationRule("ItemA OR NOT ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement Law"),
            new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity Law"),
            new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment Law"),
            //NOT Laws (3/3)
            new SimplificationRule("NOT 0", "1", "NOT 0 is 1"),
            new SimplificationRule("NOT 1", "0", "NOT 1 is 0"),
            new SimplificationRule("NOT NOT ItemA", "ItemA", "Involution Law"),
            //De Morgan's Laws (0/4)
            //new SimplificationRule("NOT (ItemA AND ItemB AND [ItemsA])", "(NOT ItemA) OR (NOT ItemB) OR [(NOT ItemsA)]", "De Morgan's Law"),
            //new SimplificationRule("NOT (ItemA OR ItemB OR [ItemsA])", "(NOT ItemA) AND (NOT ItemB) AND [(NOT ItemsA)]", "De Morgan's Law"),
            //new SimplificationRule("(NOT ItemA) OR (NOT ItemB) OR [(NOT ItemsA)] OR [ItemsB]", "NOT (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's Law"),
            //new SimplificationRule("(NOT ItemA) AND (NOT ItemB) AND [(NOT ItemsA)] AND [ItemsB]", "NOT (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's Law"),
            //Absorption Laws (0/2)
            //new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption Law"),
            //new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption Law"),
            //Distributive Laws (0/4)
            //new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive Law"),
            //new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive Law"),
            //new SimplificationRule("(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)] OR [ItemsC]", "(ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])) OR [ItemsC]", "Distributive Law"),
            //new SimplificationRule("(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)] AND [ItemsC]", "(ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])) AND [ItemsC]", "Distributive Law"),
        };
    }

    private static IEnumerable<SimplificationRule> GenerateSimplificationRules() {
        return new[] {
            //AND Laws
            new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent Law"),
            new SimplificationRule("ItemA AND NOT ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement Law"),
            new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment Law"),
            new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity Law"),
            //OR Laws
            new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent Law"),
            new SimplificationRule("ItemA OR NOT ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement Law"),
            new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity Law"),
            new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment Law"),
            //NOT Laws
            new SimplificationRule("NOT 0", "1", "NOT 0 is 1"),
            new SimplificationRule("NOT 1", "0", "NOT 1 is 0"),
            new SimplificationRule("NOT NOT ItemA", "ItemA", "Involution Law"),
            //De Morgan's Laws
            new SimplificationRule("NOT (ItemA AND ItemB AND [ItemsA])", "(NOT ItemA) OR (NOT ItemB) OR [(NOT ItemsA)]", "De Morgan's Law"),
            new SimplificationRule("NOT (ItemA OR ItemB OR [ItemsA])", "(NOT ItemA) AND (NOT ItemB) AND [(NOT ItemsA)]", "De Morgan's Law"),
            new SimplificationRule("(NOT ItemA) OR (NOT ItemB) OR [(NOT ItemsA)] OR [ItemsB]", "NOT (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's Law"),
            new SimplificationRule("(NOT ItemA) AND (NOT ItemB) AND [(NOT ItemsA)] AND [ItemsB]", "NOT (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's Law"),
            //Absorption Laws 
            new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption Law"),
            new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption Law"),
            //Distributive Laws
            new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive Law"),
            new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive Law"),
            new SimplificationRule("(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)] OR [ItemsC]", "(ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])) OR [ItemsC]", "Distributive Law"),
            new SimplificationRule("(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)] AND [ItemsC]", "(ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])) AND [ItemsC]", "Distributive Law"),
        };
    }
}