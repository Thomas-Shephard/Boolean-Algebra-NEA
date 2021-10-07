using System;

namespace BooleanAlgebra.Syntax.Operands {
    public class VariableOperand : SyntaxItem {
        public string Identifier { get; }

        public VariableOperand(string identifier) {
            Identifier = identifier;
        }

        public override string ToString() {
            return $"'{Identifier}'";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is VariableOperand otherVariableOperand
                   && Identifier == otherVariableOperand.Identifier;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Identifier);
        }
    }
}