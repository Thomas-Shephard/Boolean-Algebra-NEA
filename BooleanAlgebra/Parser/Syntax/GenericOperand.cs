namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// Holds additional information about an operand that is used in the simplification rule substitution process.
/// A generic operand is one that is substituted with a given syntax item, a repeating generic operand is optional and can be substituted any number of times.
/// </summary>
public class GenericOperand : Operand {
    /// <summary>
    /// Whether the generic operand can be substituted any number of times (including zero times).
    /// </summary>
    public bool IsRepeating { get; }

    /// <summary>
    /// Initialises a new generic operand with the given identifier, value and whether the generic operand can be substituted any number of times.
    /// </summary>
    /// <param name="identifier">The type that the generic operand is of.</param>
    /// <param name="value">The value that the generic operand represents.</param>
    /// <param name="isRepeating">Whether the generic operand can be substituted any number of times.</param>
    public GenericOperand(Identifier identifier, string value, bool isRepeating) : base(identifier, value) {
        IsRepeating = isRepeating;
    }

    /// <summary>
    /// Initialises a new generic operand with the same properties as the given generic operand.
    /// </summary>
    /// <param name="genericOperand">The generic operand to copy the properties from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="genericOperand"/> is null.</exception>
    private GenericOperand(GenericOperand genericOperand) : base(genericOperand) {
        //Create a shallow copy of the object by initialising a new object with the same property values.
        if (genericOperand is null) throw new ArgumentNullException(nameof(genericOperand));
        IsRepeating = genericOperand.IsRepeating;
    }
    
    public override ISyntaxItem ShallowClone() {
        //Create a new generic operand object with the same properties as the current object.
        return new GenericOperand(this);
    }

    public override int GetCost() {
        //The get cost method is only usable on syntax items that have been substituted.
        throw new NotSupportedException("GetCost() is not supported for syntax items that have not been substituted.");
    }

    public override string GetStringRepresentation(int higherLevelPrecedence = 0) {
        //The get string representation method is only usable on syntax items that are displayed in the UI.
        throw new NotSupportedException("GetStringRepresentation() is not supported for syntax items that are not displayed UI.");
    }

    public override bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false; //If the other syntax item is null, it cannot be equal to this generic operand.
        if (ReferenceEquals(this, other)) return true;  //If the other syntax item is this generic operand, it is equal to this generic operand.

        //The syntax items are only equal if they are both generic operands and have properties that are equal to each other.
        return other is GenericOperand otherGenericOperand
            && Identifier.Equals(otherGenericOperand.Identifier)
            && Value == otherGenericOperand.Value
            && IsRepeating == otherGenericOperand.IsRepeating;
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this generic operand:
        //  1. If the other object is a generic operand.
        //  2. If the other generic operand satisfies the equality comparison.
        return obj is GenericOperand otherGenericOperand && Equals(otherGenericOperand);
    }
    
    public override int GetHashCode() {
        //The hash code is not supported for syntax items to ensure that equality is not based off the order of any child nodes.
        throw new NotSupportedException("GetHashCode() is not supported for syntax items.");
    }
}