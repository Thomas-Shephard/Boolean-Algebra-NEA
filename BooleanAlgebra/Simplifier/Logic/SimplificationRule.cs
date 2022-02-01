﻿namespace BooleanAlgebra.Simplifier.Logic;
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
    public SimplificationTraversalOrder SimplificationTraversalOrder { get; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPostSimplification { get; }
    /// <summary>
    /// 
    /// </summary>
    public bool AllowMultiple { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="leftHandSide">The pattern that must be matched for the simplification to be valid.</param>
    /// <param name="rightHandSide">The pattern that is substituted to simplify the syntax tree.</param>
    /// <param name="description">The reason why the simplification can occur.</param>
    /// <param name="precedence"></param>
    /// <param name="simplificationPattern"></param>
    /// <param name="simplificationOrder"></param>
    /// <param name="allowMultiple"></param>
    private SimplificationRule(string leftHandSide, string rightHandSide, string description, int precedence, SimplificationTraversalOrder simplificationTraversalOrder, bool isPostSimplification, bool allowMultiple) {
        //Lex and then parse the left hand side and store the resultant syntax tree.
        LeftHandSide = new Parser.Parser(new Lexer.Lexer(leftHandSide, true).Lex(), true).Parse();
        //Lex and then parse the right hand side and store the resultant syntax tree.
        RightHandSide = new Parser.Parser(new Lexer.Lexer(rightHandSide, true).Lex(), true).Parse();
        Description = description;
        Precedence = precedence;
        SimplificationTraversalOrder = simplificationTraversalOrder;
        IsPostSimplification = isPostSimplification;
        AllowMultiple = allowMultiple;
    }

    /// <summary>
    /// Stores a cache of all the available simplification rules.
    /// </summary>
    private static SimplificationRule[]? _simplificationRules;

    /// <summary>
    /// Provides a collection of all the available simplification rules.
    /// </summary>
    /// <returns>A collection of all the available simplification rules.</returns>
    public static IEnumerable<SimplificationRule> GetSimplificationRules() {
        //If the cache is empty, generate the simplification rules first.
        return _simplificationRules ??= GenerateSimplificationRules();
    }

    /// <summary>
    /// Generates a new collection of all the available simplification rules that can be used to simplify a given syntax tree.
    /// </summary>
    /// <returns>A new collection of all the available simplification rules that can be used to simplify a given syntax tree.</returns>
    private static SimplificationRule[] GenerateSimplificationRules() {
        return new[] {
            #region AND Laws
            //A AND A --> A
            new SimplificationRule("ItemA AND ItemA AND [ItemsA]", "ItemA AND [ItemsA]", "Idempotent law", 3, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //A AND NOT A --> 0
            new SimplificationRule("ItemA AND !ItemA AND [ItemsA]", "0 AND [ItemsA]", "Complement law", 1, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //A AND 0 --> 0
            new SimplificationRule("ItemA AND 0 AND [ItemsA]", "0", "Annulment law", 0, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //A AND 1 --> A
            new SimplificationRule("ItemA AND 1 AND [ItemsA]", "ItemA AND [ItemsA]", "Identity law", 3, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            #endregion
            #region OR Laws
            //A OR A --> A
            new SimplificationRule("ItemA OR ItemA OR [ItemsA]", "ItemA OR [ItemsA]", "Idempotent law", 3, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //A OR NOT A --> 1
            new SimplificationRule("ItemA OR !ItemA OR [ItemsA]", "1 OR [ItemsA]", "Complement law", 1, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //A OR 0 --> A
            new SimplificationRule("ItemA OR 0 OR [ItemsA]", "ItemA OR [ItemsA]", "Identity law", 3, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //A OR 1 --> 1
            new SimplificationRule("ItemA OR 1 OR [ItemsA]", "1", "Annulment law", 0, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            #endregion
            #region NOT Laws
            //NOT 0 --> 1
            new SimplificationRule("!0", "1", "Negation law", 1, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //NOT 1 --> 0
            new SimplificationRule("!1", "0", "Negation law", 1, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //NOT NOT A --> A
            new SimplificationRule("!!ItemA", "ItemA", "Involution law", 0, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            #endregion
            #region Abosorpion Laws
            //A AND (A OR B) --> A
            new SimplificationRule("ItemA AND (ItemA OR [ItemsA]) AND [ItemsB]", "ItemA AND [ItemsB]", "Absorption law", 2, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //A OR (A AND B) --> A
            new SimplificationRule("ItemA OR (ItemA AND [ItemsA]) OR [ItemsB]", "ItemA OR [ItemsB]", "Absorption law", 2, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //NOT A OR (A AND B) --> NOT A OR B
            new SimplificationRule("!ItemA OR (ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "!ItemA OR (ItemB AND [ItemsA]) OR [ItemsB]", "Absorption law", 2, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //NOT A AND (A OR B) --> NOT A AND B
            new SimplificationRule("!ItemA AND (ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "!ItemA AND (ItemB OR [ItemsA]) AND [ItemsB]", "Absorption law", 2, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            #endregion
            #region De Morgan's Laws
            //NOT (A AND B) --> NOT A OR NOT B
            new SimplificationRule("!(ItemA AND ItemB AND [ItemsA])", "!ItemA OR !ItemB OR [!ItemsA]", "De Morgan's law", 4, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //NOT (A OR B) --> NOT A AND NOT B
            new SimplificationRule("!(ItemA OR ItemB OR [ItemsA])", "!ItemA AND !ItemB AND [!ItemsA]", "De Morgan's law", 4, SimplificationTraversalOrder.OUTSIDE_IN, false, false),
            //NOT A OR NOT B --> NOT (A AND B)
            new SimplificationRule("!ItemA OR !ItemB OR [!ItemsA] OR [ItemsB]", "!(ItemA AND ItemB AND [ItemsA]) OR [ItemsB]", "De Morgan's law", 0, SimplificationTraversalOrder.OUTSIDE_IN, true, false),
            //NOT A AND NOT B --> NOT (A OR B)
            new SimplificationRule("!ItemA AND !ItemB AND [!ItemsA] AND [ItemsB]", "!(ItemA OR ItemB OR [ItemsA]) AND [ItemsB]", "De Morgan's law", 0, SimplificationTraversalOrder.OUTSIDE_IN, true, false),
            #endregion
            #region Distributive Laws
            //(A AND B) OR (A AND C) --> A OR (B AND C)
            new SimplificationRule("(ItemA AND ItemB AND [ItemsB]) OR (ItemA AND ItemC AND [ItemsC]) OR [ItemsA]", "[ItemsA] OR (ItemA AND ((ItemB AND [ItemsB]) OR (ItemC AND [ItemsC])))", "Distributive law", 5, SimplificationTraversalOrder.INSIDE_OUT, false, true),
            //(A OR B) AND (A OR C) --> A AND (B OR C)
            new SimplificationRule("(ItemA OR ItemB OR [ItemsB]) AND (ItemA OR ItemC OR [ItemsC]) AND [ItemsA]", "[ItemsA] AND (ItemA OR ((ItemB OR [ItemsB]) AND (ItemC OR [ItemsC])))", "Distributive law", 5, SimplificationTraversalOrder.INSIDE_OUT, false, true),
            //A AND (B OR C) --> (A AND B) OR (A AND C)
            new SimplificationRule("ItemA AND [ItemsA] AND (ItemB OR ItemC OR [ItemsB])", "(ItemA AND [ItemsA] AND ItemB) OR (ItemA AND [ItemsA] AND ItemC) OR [(ItemA AND [ItemsA] AND ItemsB)]", "Distributive law", 5, SimplificationTraversalOrder.INSIDE_OUT, false, true),
            //A OR (B AND C) --> (A OR B) AND (A OR C)
            new SimplificationRule("ItemA OR [ItemsA] OR (ItemB AND ItemC AND [ItemsB])", "(ItemA OR [ItemsA] OR ItemB) AND (ItemA OR [ItemsA] OR ItemC) AND [(ItemA OR [ItemsA] OR ItemsB)]", "Distributive law", 5, SimplificationTraversalOrder.INSIDE_OUT, false, true),
            #endregion
        };
    }
}