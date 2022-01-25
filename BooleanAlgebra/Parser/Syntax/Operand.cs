namespace BooleanAlgebra.Parser.Syntax; 
public class Operand : ISyntaxItem {
    public string Value { get; }
    public Identifier Identifier { get; }

    public Operand(string value, Identifier identifier) {
        Value = value;
        Identifier = identifier;
    }
    
    public int GetCost() {
        return 1;
    }
    
    public string GetStringRepresentation(int higherLevelPrecedence = 0) {
        return Value;
    }

    public virtual bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return other is Operand operand and not GenericOperand
            && Value == operand.Value
            && Identifier.Equals(operand.Identifier);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as Operand);
    }
    
    public override int GetHashCode() {
        return HashCode.Combine(Value, Identifier);
    }
}