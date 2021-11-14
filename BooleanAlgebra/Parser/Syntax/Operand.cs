namespace BooleanAlgebra.Parser.Syntax;
public class Operand : SyntaxItem {
    public override uint GetCost() {
        return 1;
    }

    public override string Value { get; }
    public sealed override List<SyntaxItem> DaughterItems { get; set; }

    public override Operand Clone() {
        return new Operand(Value);
    }

    public Operand(string value) {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        DaughterItems = new List<SyntaxItem>();
    }

    public override string ToString() {
        return Value;
    }

    public override bool Equals(SyntaxItem? other) {
        return other is Operand otherOperand1
               && Value == otherOperand1.Value;
    }

    public override bool Equals(object? obj) {
        return Equals(obj as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value);
    }
}