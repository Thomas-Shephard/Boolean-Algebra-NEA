using System;

namespace BooleanAlgebra.Parser.Syntax.Operands {
    public class BooleanOperand : SyntaxItem {
        public string Value { get; }

        public BooleanOperand(string value) {
            Value = value.ToUpper();
        }

        public override string ToString() {
            return $"'{Value}'";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is BooleanOperand otherBooleanOperand
                   && Value == otherBooleanOperand.Value;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value);
        }
    }
}