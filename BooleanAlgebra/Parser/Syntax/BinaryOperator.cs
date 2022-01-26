namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// 
/// </summary>
public class BinaryOperator : IMultiChildSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem[] Children { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="children"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BinaryOperator(Identifier identifier, ISyntaxItem[] children) {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        Children = children ?? throw new ArgumentNullException(nameof(children));
    }
    
    public int GetCost() {
        //To calculate the cost of the binary operator, we need to sum the cost of all the daughter items together and add the cost of this node.
        //To calculate the base cost of this node we need to calculate how many times the operator is applied to the daughter items.
        //For example if the expression is 'a + b + c', the base cost is 2. If the expression is 'a + b + c + d', the base cost is 3.
        //The base cost is calculated by subtracting one from the number of child nodes this node has.
        //The cost of the daughter items can be calculated by recursively calling the GetCost() method on each of the child items and summing them together.
        return Children.Length - 1 + Children.Sum(childNode => childNode.GetCost());
    }
    
    public string GetStringRepresentation(int higherLevelPrecedence = 0) {
        //Parentheses are only required if an operator with a lower precedence is nested inside an operator with a higher precedence.
        //For example in the expression '(a + b) . c', the tree structure will look like the following:
        //        .
        //       / \
        //      +   c
        //     / \
        //    a   b
        //The operator '+' has a lower precedence that the '.' operator it is nested inside, so parentheses are required to encapsulate the '+' operator.
        bool parenthesesRequired = higherLevelPrecedence > Identifier.Precedence;
        
        //Use a string builder as strings are immutable and therefore each concatenation of a pre-existing string creates a new string under the hood. 
        StringBuilder stringBuilder = new();
        if (parenthesesRequired) {
            //Start the expression with an opening parenthesis only if parentheses are required.
            stringBuilder.Append('(');
        }

        //Iterate over all the child nodes and add them to the expression.
        for (int i = 0; i < Children.Length; i++) {
            if (i > 0) {
                //On the first iteration, the operators name is not added to the expression.
                //If this check was removed, the expression would be '+ a + b' instead of 'a + b'.
                stringBuilder.Append(' ').Append(Identifier.Name).Append(' ');
            }
            //Recursively call the GetStringRepresentation() method on the child node.
            //This will return the string representation of the child node and add it to the expression.
            stringBuilder.Append(Children[i].GetStringRepresentation(Identifier.Precedence));
        }
        
        if (parenthesesRequired) {
            //Close the expression with a closing parenthesis only if parentheses are required.
            stringBuilder.Append(')');
        }
        
        return stringBuilder.ToString();
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false; //If the syntax item is null, it cannot be equal to this binary operator.
        if (ReferenceEquals(this, other)) return true;  //If the syntax item is this binary operator, it is equal to this binary operator.
        
        //The syntax item is only equal to this syntax item if it is a syntax item and if all the properties are equal to each other.
        //The child nodes are compared using a custom algorithm because the order of the child nodes is unimportant.
        return other is BinaryOperator otherBinaryOperator
            && Identifier.Equals(otherBinaryOperator.Identifier)
            && Children.SyntaxItemsEqualIgnoreOrder(otherBinaryOperator.Children);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;   //If the object is null, it cannot be equal to this object.
        if (ReferenceEquals(this, obj)) return true;    //If the object is this object, it is equal to this object.
        
        //Compare the other object as a binary operator with a more specific comparison.
        //If the other object is not a binary operator, the comparison will fail.
        return Equals(obj as BinaryOperator);
    }
    
    public override int GetHashCode() {
        //The hash code is not supported for syntax items to ensure that equality is not based off the order of any child nodes.
        throw new NotSupportedException("GetHashCode() is not supported for syntax items.");
    }
}