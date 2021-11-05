namespace BooleanAlgebra.Parser.Syntax; 

public class GenericOperand : Operand {
    public GenericOperand(string value) : base(value) { }
    
    public override string ToString() {
        return $"Generic '{Value}'";
    }

    public override bool Equals(SyntaxItem? other) {
        return other is GenericOperand genericOperand 
               && Value == genericOperand.Value;
    }

    public override bool Equals(object? obj) {
        return Equals(obj as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value);
    }
}