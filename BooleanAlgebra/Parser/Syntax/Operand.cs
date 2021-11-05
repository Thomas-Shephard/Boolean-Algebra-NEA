namespace BooleanAlgebra.Parser.Syntax;
public class Operand : SyntaxItem {
    public override string Value { get; }
    public bool IsGeneric { get; }
    public sealed override List<SyntaxItem> DaughterItems { get; set; }

    public override Operand Clone() {
        return new Operand(Value, IsGeneric);
    }

    public Operand(string value, bool isGeneric = false) {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsGeneric = isGeneric;
        DaughterItems = new List<SyntaxItem>();
    }

    public override string ToString() {
        return IsGeneric
            ? $"Generic '{Value}'"
            : $"{Value}";
    }

    public override bool Equals(SyntaxItem? other) {
        return other is Operand otherOperand1
            && Value == otherOperand1.Value
            && IsGeneric == otherOperand1.IsGeneric;
    }

    public override bool Equals(object? obj) {
        return Equals(obj as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value, IsGeneric);
    }
}