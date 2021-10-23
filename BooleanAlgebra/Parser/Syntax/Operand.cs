namespace BooleanAlgebra.Parser.Syntax;
public class Operand : SyntaxItem {
    public override string Value { get; }
    public bool IsGeneric { get; }

    public Operand(string value, bool isGeneric) {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsGeneric = isGeneric;
    }

    public override string ToString() {
        return IsGeneric
            ? $"Generic '{Value}'"
            : $"'{Value}'";
    }

    public override bool Equals(SyntaxItem? other) {
        return other is Operand otherOperand
            && Value == otherOperand.Value
            && IsGeneric == otherOperand.IsGeneric;
    }

    public override bool Equals(object? obj) {
        return Equals(obj as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value, IsGeneric);
    }
}