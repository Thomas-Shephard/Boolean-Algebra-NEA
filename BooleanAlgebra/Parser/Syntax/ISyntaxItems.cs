namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// Holds the generalised information about a node in the syntax tree.
/// </summary>
/// <example>All nodes in the syntax tree are of this base type.</example>
public interface ISyntaxItem : IEquatable<ISyntaxItem> {
    /// <summary>
    /// The cost that each node in the syntax tree has.
    /// (In the case of the binary operator this is the cost each node would be if it were represented as a binary operator with only two child nodes)
    /// </summary>
    protected const int NodeCost = 1;
    /// <summary>
    /// Stores the identifier about the type of a node.
    /// This means that the node can be identified as being of a particular type. (e.g. OR, AND, NOT, etc.)
    /// </summary>
    public Identifier Identifier { get; }
    
    /// <summary>
    /// Returns a shallow copy of the specified syntax item.
    /// </summary>
    /// <returns>A shallow copy of the specified syntax item.</returns>
    public ISyntaxItem ShallowClone();
    
    /// <summary>
    /// Returns the sum of the cost of a node and that of its child nodes.
    /// </summary>
    /// <returns>The cost of the node and its child nodes.</returns>
    public int GetCost();
    
    /// <summary>
    /// Returns a string representation of the queried syntax item.
    /// The string representation only includes parentheses when excluding them would alter the meaning of the expression.
    /// </summary>
    /// <param name="higherLevelPrecedence">The precedence of the syntax item that called the method.</param>
    /// <returns>A string representation of the queried syntax item.</returns>
    public string GetStringRepresentation(int higherLevelPrecedence = 0);
}

/// <summary>
/// Holds the information about a node in the syntax tree that has a single child node.
/// </summary>
/// <example>The repeating operator and unary operator nodes are of this type.</example>
public interface ISingleChildSyntaxItem : ISyntaxItem {
    /// <summary>
    /// Stores the singular child node that the node has.
    /// </summary>
    public ISyntaxItem ChildNode { get; set; }
}

/// <summary>
/// Holds the information about a node in the syntax tree that has at least two child nodes.
/// </summary>
/// <example>The binary operator node is of this type.</example>
public interface IMultiChildSyntaxItem : ISyntaxItem {
    /// <summary>
    /// Stores the multiple child nodes that the node has.
    /// </summary>
    public ISyntaxItem[] ChildNodes { get; set; }
}