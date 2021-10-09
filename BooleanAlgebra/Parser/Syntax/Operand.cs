using System;

namespace BooleanAlgebra.Parser.Syntax {
    public class Operand : SyntaxItem {
        public string Value { get; }

        public Operand(string value) {
            Value = value.ToUpper();
        }

        public override string ToString() {
            return $"'{Value}'";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is Operand otherOperand
                   && Value == otherOperand.Value;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value);
        }
    }
}