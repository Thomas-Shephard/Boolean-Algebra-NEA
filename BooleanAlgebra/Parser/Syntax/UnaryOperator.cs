namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// Holds the information about the type that a unary operator is of and the child node that the unary operator is applied to.
/// A unary operator is an operator that has a singular child node.
/// </summary>
public class UnaryOperator : ISingleChildSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem ChildNode { get; set; }

    /// <summary>
    /// Initialises a new unary operator with the given identifier and child node.
    /// </summary>
    /// <param name="identifier">The type that the unary operator is of.</param>
    /// <param name="childNode">The child node that is nested inside the unary operator.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="identifier"/> or <paramref name="childNode"/> is null.</exception>
    public UnaryOperator(Identifier identifier, ISyntaxItem childNode) {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        ChildNode = childNode ?? throw new ArgumentNullException(nameof(childNode));
    }

    /// <summary>
    /// Initialises a new unary operator with the same properties as the given unary operator.
    /// </summary>
    /// <param name="unaryOperator">The unary operator to copy the properties from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="unaryOperator"/> is null.</exception>
    private UnaryOperator(UnaryOperator unaryOperator) {
        //Create a shallow copy of the object by initialising a new object with the same property values.
        if (unaryOperator is null) throw new ArgumentNullException(nameof(unaryOperator));
        Identifier = unaryOperator.Identifier;
        ChildNode = unaryOperator.ChildNode;
    }
    
    public ISyntaxItem ShallowClone() {
        //Create a new unary operator object with the same properties as the current object.
        return new UnaryOperator(this);
    }
    
    public int GetCost() {
        //The cost of a unary operator is equal to the cost of a node plus the recursive cost of the child node.
        return ISyntaxItem.NodeCost + ChildNode.GetCost();
    }
    
    public string GetStringRepresentation(int higherLevelPrecedence = 0) {
        #region Parentheses Explanation
        //Parentheses are only required if an operator with a lower precedence is nested inside an operator with a higher precedence.
        //For example in the expression '(a + b) . c', the tree structure will look like the following:
        //        .
        //       / \
        //      +   c
        //     / \
        //    a   b
        //The operator '+' has a lower precedence that the '.' operator it is nested inside, so parentheses are required to encapsulate the '+' operator.
        #endregion
        bool parenthesesRequired = higherLevelPrecedence > Identifier.Precedence;
        //Gets the string representation of the child node.
        string childString = ChildNode.GetStringRepresentation(Identifier.Precedence);
        //The string representation of a unary operator is the operator's identifier followed by the child's string representation.
        //If parentheses are required, the string representation is enclosed in parentheses.
        return parenthesesRequired 
            ? $"({Identifier.Name}{childString})" 
            : $"{Identifier.Name}{childString}";
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false; //If the other syntax item is null, it cannot be equal to this unary operator.
        if (ReferenceEquals(this, other)) return true;  //If the other syntax item is this unary operator, it is equal to this unary operator.
        
        //The syntax items are only equal if they are both unary operators and have properties that are equal to each other.
        return other is UnaryOperator otherUnaryOperator
            && Identifier.Equals(otherUnaryOperator.Identifier) 
            && ChildNode.Equals(otherUnaryOperator.ChildNode);
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this unary operator:
        //  1. If the other object is a unary operator.
        //  2. If the other unary operator satisfies the equality comparison.
        return obj is UnaryOperator otherUnaryOperator && Equals(otherUnaryOperator);
    }
    
    public override int GetHashCode() {
        //The hash code is not supported for syntax items to ensure that equality is not based off the order of any child nodes.
        throw new NotSupportedException("GetHashCode() is not supported for syntax items.");
    }
}