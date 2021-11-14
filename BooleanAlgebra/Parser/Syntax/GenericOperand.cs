namespace BooleanAlgebra.Parser.Syntax; 

public class GenericOperand : Operand {
    public bool IsRepeating { get; }

    public GenericOperand(string value) : base(value) {
        IsRepeating = value.StartsWith("Items");
    }
    
    public override string ToString() {
        return $"Generic '{Value}'";
    }

    public override bool Equals(SyntaxItem? other) {
        return other is GenericOperand genericOperand 
               && Value == genericOperand.Value
               && IsRepeating == genericOperand.IsRepeating;
    }

    public override bool Equals(object? obj) {
        return Equals(obj as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value, IsRepeating);
    }
}