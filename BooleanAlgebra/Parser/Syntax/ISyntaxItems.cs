namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// Holds the generalised information about a node in the syntax tree.
/// </summary>
/// <example>All nodes in the syntax tree are of this base type.</example>
public interface ISyntaxItem : IEquatable<ISyntaxItem> {
    /// <summary>
    /// Stores the identifier that the node was generated with.
    /// This means that the node can be identified as being of a particular type. (e.g. OR, AND, NOT, etc.)
    /// </summary>
    public Identifier Identifier { get; }
    /// <summary>
    /// Returns the cost of the node and its children.
    /// The cost is calculated by summing the number of nodes in the tree.
    /// </summary>
    /// <returns>The cost of the node and its children.</returns>
    public int GetCost();
    /// <summary>
    /// Returns a string representation of the queried syntax item.
    /// The string representation only includes parentheses when excluding them would change the meaning of the expression.
    /// </summary>
    /// <param name="higherLevelPrecedence">The precedence of the syntax item that called this method.</param>
    /// <returns>A string representation of the queried syntax item.</returns>
    public string GetStringRepresentation(int higherLevelPrecedence = 0);

    public ISyntaxItem ShallowClone();
}

/// <summary>
/// Holds the information about a node in the syntax tree that has a single child node.
/// </summary>
/// <example>The repeating operator and unary operator nodes are of this type.</example>
public interface ISingleChildSyntaxItem : ISyntaxItem {
    /// <summary>
    /// Stores the singular child node that the node has.
    /// </summary>
    public ISyntaxItem Child { get; set; }
}

/// <summary>
/// Holds the information about a node in the syntax tree that has at least two child nodes.
/// </summary>
/// <example>The binary operator node is of this type.</example>
public interface IMultiChildSyntaxItem : ISyntaxItem {
    /// <summary>
    /// Stores the multiple child nodes that the node has.
    /// </summary>
    public ISyntaxItem[] Children { get; set; }
}