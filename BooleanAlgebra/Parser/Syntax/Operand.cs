namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// Holds the information about the type that an operand is of and the value of the operand.
/// An operand is a leaf node and has no child nodes.
/// </summary>
public class Operand : ISyntaxItem {
    public Identifier Identifier { get; }
    /// <summary>
    /// The value that the operand represents.
    /// </summary>
    /// <example>The boolean literal '0' or '1'. The variable name 'x' or 'y'.</example>
    protected string Value { get; }

    /// <summary>
    /// Initialises a new operand with the given identifier and value.
    /// </summary>
    /// <param name="identifier">The type that the operand is of.</param>
    /// <param name="value">The value that the operand represents.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="identifier"/> or <paramref name="value"/> is null.</exception>
    public Operand(Identifier identifier, string value) {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Initialises a new operand with the same properties as the given operand.
    /// </summary>
    /// <param name="operand">The operand to copy the properties from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="operand"/> is null.</exception>
    protected Operand(Operand operand) {
        //Create a shallow copy of the object by initialising a new object with the same property values.
        if (operand is null) throw new ArgumentNullException(nameof(operand));
        Value = operand.Value;
        Identifier = operand.Identifier;
    }
    
    public virtual ISyntaxItem ShallowClone() {
        //Create a new operand object with the same properties as the current object.
        return new Operand(this);
    }
    
    public virtual int GetCost() {
        //The cost of an operand is equal to the cost of a node.
        return ISyntaxItem.NodeCost;
    }
    
    public virtual string GetStringRepresentation(int higherLevelPrecedence = 0) {
        //The string representation of an operand is the value of the operand.
        return Value;
    }

    public virtual bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false; //If the other syntax item is null, it cannot be equal to this operand.
        if (ReferenceEquals(this, other)) return true;  //If the other syntax item is this operand, it is equal to this operand.

        //The syntax items are only equal if they are both operands and have properties that are equal to each other.
        return other is Operand otherOperand 
            && Identifier.Equals(otherOperand.Identifier)
            && Value == otherOperand.Value;
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this operand:
        //  1. If the other object is an operand.
        //  2. If the other operand satisfies the equality comparison.
        return obj is Operand otherOperand && Equals(otherOperand);
    }
    
    public override int GetHashCode() {
        //The hash code is not supported for syntax items to ensure that equality is not based off the order of any child nodes.
        throw new NotSupportedException("GetHashCode() is not supported for syntax items.");
    }
}