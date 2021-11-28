namespace BooleanAlgebra.Parser.Syntax; 
public class GenericOperand : Operand {
    public bool IsRepeating { get; }

    public GenericOperand(string value, Identifier identifier) : base(value, identifier) {
        IsRepeating = value.StartsWith("Items");
    }

    public override bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return other is GenericOperand otherGenericOperand
            && Value == otherGenericOperand.Value
            && IsRepeating == otherGenericOperand.IsRepeating
            && Identifier.Equals(otherGenericOperand.Identifier);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as GenericOperand);
    }
    
    public override int GetHashCode() {
        return HashCode.Combine(Value, Identifier, IsRepeating);
    }
}