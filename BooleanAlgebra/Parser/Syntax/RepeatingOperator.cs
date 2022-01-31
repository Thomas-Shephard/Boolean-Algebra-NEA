namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// Holds  the information about the type that a repeating operator is of and the child node that is nested inside the repeating operator.
/// A repeating operator is used within the simplification rule substitution where an expression can be substituted a number of times.
/// </summary>
public class RepeatingOperator : ISingleChildSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem ChildNode { get; set; }

    /// <summary>
    /// Initialises a new repeating operator with the given identifier and child node.
    /// </summary>
    /// <param name="identifier">The type that the repeating operator is of.</param>
    /// <param name="childNode">The child node that is nested inside the repeating operator</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="identifier"/> or <paramref name="childNode"/> is null.</exception>
    public RepeatingOperator(Identifier identifier, ISyntaxItem childNode) {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        ChildNode = childNode ?? throw new ArgumentNullException(nameof(childNode));
    }

    /// <summary>
    /// Initialises a new repeating operator with the same properties as the given repeating operator.
    /// </summary>
    /// <param name="repeatingOperator">The repeating operator to copy the properties from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="repeatingOperator"/> is null.</exception>
    private RepeatingOperator(RepeatingOperator repeatingOperator) {
        //Create a shallow copy of the object by initialising a new object with the same property values.
        if (repeatingOperator is null) throw new ArgumentNullException(nameof(repeatingOperator));
        Identifier = repeatingOperator.Identifier;
        ChildNode = repeatingOperator.ChildNode;
    }
    
    public ISyntaxItem ShallowClone() {
        //Create a new repeating operator object with the same properties as the current object.
        return new RepeatingOperator(this);
    }
    
    public int GetCost() {
        //The get cost method is only usable on syntax items that have been substituted.
        throw new NotSupportedException("GetCost() is not supported for syntax items that have not been substituted.");
    }
    
    public string GetStringRepresentation(int higherLevelPrecedence = 0) {
        //The get string representation method is only usable on syntax items that are displayed in the UI.
        throw new NotSupportedException("GetStringRepresentation() is not supported for syntax items that are not displayed UI.");
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false; //If the other syntax item is null, it cannot be equal to this repeating operator.
        if (ReferenceEquals(this, other)) return true;  //If the other syntax item is this repeating operator, it is equal to this repeating operator.
        
        //The syntax items are only equal if they are both repeating operators and have properties that are equal to each other.
        return other is RepeatingOperator otherRepeatingOperator
            && Identifier.Equals(otherRepeatingOperator.Identifier) 
            && ChildNode.Equals(otherRepeatingOperator.ChildNode);
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this repeating operator:
        //  1. If the other object is a repeating operator.
        //  2. If the other repeating operator satisfies the equality comparison.
        return obj is RepeatingOperator otherRepeatingOperator && Equals(otherRepeatingOperator);
    }
    
    public override int GetHashCode() {
        //The hash code is not supported for syntax items to ensure that equality is not based off the order of any child nodes.
        throw new NotSupportedException("GetHashCode() is not supported for syntax items.");
    }
}