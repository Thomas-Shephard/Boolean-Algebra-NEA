namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// 
/// </summary>
public class SimplificationRule {
    /// <summary>
    /// 
    /// </summary>
    public ISyntaxItem LeftHandSide { get; }
    /// <summary>
    /// 
    /// </summary>
    public ISyntaxItem RightHandSide { get; }
    /// <summary>
    /// 
    /// </summary>
    public string Description { get; }
    /// <summary>
    /// 
    /// </summary>
    public int Precedence { get; }
    /// <summary>
    /// 
    /// </summary>
    public SimplificationTreeTraversalOrder SimplificationPattern { get; }
    /// <summary>
    /// 
    /// </summary>
    public SimplificationOrder SimplificationOrder { get; }
    /// <summary>
    /// 
    /// </summary>
    public bool AllowMultiple { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="leftHandSide"></param>
    /// <param name="rightHandSide"></param>
    /// <param name="description"></param>
    /// <param name="precedence"></param>
    /// <param name="simplificationPattern"></param>
    /// <param name="simplificationOrder"></param>
    /// <param name="allowMultiple"></param>
    private SimplificationRule(string leftHandSide, string rightHandSide, string description, int precedence, SimplificationTreeTraversalOrder simplificationPattern, SimplificationOrder simplificationOrder, bool allowMultiple) {
        //Lex and then parse the left hand side and store the resultant syntax tree.
        LeftHandSide = new Parser.Parser(new Lexer.Lexer(leftHandSide, true).Lex(), true).Parse();
        //Lex and then parse the right hand side and store the resultant syntax tree.
        RightHandSide = new Parser.Parser(new Lexer.Lexer(rightHandSide, true).Lex(), true).Parse();
        Description = description;
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
            new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent law", 3, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA AND !ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement law", 1, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment law", 0, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity law", 3, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //OR Laws
            new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent law", 3, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR !ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement law", 1, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity law", 3, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment law", 0, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //NOT Laws
            new SimplificationRule("!0", "1", "Negation law", 1, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!1", "0", "Negation law", 1, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!!ItemA", "ItemA", "Involution law", 0, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //Absorption Laws
            new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption law", 2, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption law", 2, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!ItemA OR (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "!ItemA OR (ItemB AND [ItemsA]) OR [ItemsB]", "Absorption law", 2, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!ItemA AND (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "!ItemA AND (ItemB OR [ItemsA]) AND [ItemsB]", "Absorption law", 2, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            //De Morgan's Laws
            new SimplificationRule("!(ItemA AND ItemB AND [ItemsA])", "!ItemA OR !ItemB OR [!ItemsA]", "De Morgan's law", 4, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!(ItemA OR ItemB OR [ItemsA])", "!ItemA AND !ItemB AND [!ItemsA]", "De Morgan's law", 4, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.PRE, false),
            new SimplificationRule("!ItemA OR !ItemB OR [!ItemsA] OR [ItemsB]", "!(ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's law", 0, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.POST, false),
            new SimplificationRule("!ItemA AND !ItemB AND [!ItemsA] AND [ItemsB]", "!(ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's law", 0, SimplificationTreeTraversalOrder.OUTSIDE_IN, SimplificationOrder.POST, false),
            //Distributive Laws
            new SimplificationRule("(ItemA AND ItemB AND [ItemsB]) OR (ItemA AND ItemC AND [ItemsC]) OR [ItemsA]", "[ItemsA] OR (ItemA AND ((ItemB AND [ItemsB]) OR (ItemC AND [ItemsC])))", "Distributive law", 5, SimplificationTreeTraversalOrder.INSIDE_OUT, SimplificationOrder.PRE, true),
            new SimplificationRule("(ItemA OR ItemB OR [ItemsB]) AND (ItemA OR ItemC OR [ItemsC]) AND [ItemsA]", "[ItemsA] AND (ItemA OR ((ItemB OR [ItemsB]) AND (ItemC OR [ItemsC])))", "Distributive law", 5, SimplificationTreeTraversalOrder.INSIDE_OUT, SimplificationOrder.PRE, true),
            new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive law", 5, SimplificationTreeTraversalOrder.INSIDE_OUT, SimplificationOrder.PRE, true),
            new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive law", 5, SimplificationTreeTraversalOrder.INSIDE_OUT, SimplificationOrder.PRE, true),
        };
    }
}