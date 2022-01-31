namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// Holds the information about the type that a binary operator is of and the child nodes that the binary operator is applied to.
/// A binary operator is an operator that has two or more child nodes.
/// </summary>
public class BinaryOperator : IMultiChildSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem[] ChildNodes { get; set; }

    /// <summary>
    /// Initialises a new binary operator with the given identifier and child nodes.
    /// </summary>
    /// <param name="identifier">The type that the binary operator is of.</param>
    /// <param name="childNodes">The child nodes that are nested inside the binary operator.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="identifier"/> or <paramref name="childNodes"/> is null.</exception>
    public BinaryOperator(Identifier identifier, ISyntaxItem[] childNodes) {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        ChildNodes = childNodes ?? throw new ArgumentNullException(nameof(childNodes));
    }

    /// <summary>
    /// Initialises a new binary operator with the same properties as the given binary operator.
    /// </summary>
    /// <param name="binaryOperator">The binary operator to copy the properties from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="binaryOperator"/> is null.</exception>
    private BinaryOperator(BinaryOperator binaryOperator) {
        //Create a shallow copy of the object by initialising a new object with the same property values.
        if (binaryOperator is null) throw new ArgumentNullException(nameof(binaryOperator));
        Identifier = binaryOperator.Identifier;
        ChildNodes = binaryOperator.ChildNodes;
    }
    
    public ISyntaxItem ShallowClone() {
        //Create a new binary operator object with the same properties as the current object.
        return new BinaryOperator(this);
    }
    
    public int GetCost() {
        //The cost of a binary operator is equal to:
        //  1. The sum of the cost of all the recursively calculated child nodes.
        //  2. The base cost of this node (calculated by subtracting one from the number of child nodes this node has) and multiplying by the node cost.
        return ISyntaxItem.NodeCost * (ChildNodes.Length - 1) + ChildNodes.Sum(childNode => childNode.GetCost());
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
        
        //Use a string builder as strings are immutable and therefore each concatenation of a pre-existing string creates a new string under the hood. 
        StringBuilder stringBuilder = new();
        if (parenthesesRequired) {
            //Start the expression with an opening parenthesis only if parentheses are required.
            stringBuilder.Append('(');
        }

        //Iterate over all the child nodes and add them to the expression.
        for (int i = 0; i < ChildNodes.Length; i++) {
            if (i > 0) {
                //On the first iteration, the operators name is not added to the expression.
                //If this check was removed, the expression would be '+ a + b' instead of 'a + b'.
                stringBuilder.Append(' ').Append(Identifier.Name).Append(' ');
            }
            //Recursively call the GetStringRepresentation() method on the child node.
            //This will return the string representation of the child node and add it to the expression.
            stringBuilder.Append(ChildNodes[i].GetStringRepresentation(Identifier.Precedence));
        }
        
        if (parenthesesRequired) {
            //Close the expression with a closing parenthesis only if parentheses are required.
            stringBuilder.Append(')');
        }
        
        return stringBuilder.ToString();
    }
   

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false; //If the other syntax item is null, it cannot be equal to this binary operator.
        if (ReferenceEquals(this, other)) return true;  //If the other syntax item is this binary operator, it is equal to this binary operator.
        
        //The syntax items are only equal if they are both binary operators and have properties that are equal to each other.
        //The child nodes of the binary operator are not compared using the Equals() method to allow for them to be equal even if they are in a different order.
        return other is BinaryOperator otherBinaryOperator
            && Identifier.Equals(otherBinaryOperator.Identifier)
            && ChildNodes.SyntaxItemsEqualIgnoreOrder(otherBinaryOperator.ChildNodes);
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this binary operator:
        //  1. If the other object is a binary operator.
        //  2. If the other binary operator satisfies the equality comparison.
        return obj is BinaryOperator otherBinaryOperator && Equals(otherBinaryOperator);
    }
    
    public override int GetHashCode() {
        //The hash code is not supported for syntax items to ensure that equality is not based off the order of any child nodes.
        throw new NotSupportedException("GetHashCode() is not supported for syntax items.");
    }
}