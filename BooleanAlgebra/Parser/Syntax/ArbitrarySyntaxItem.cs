using System;

namespace BooleanAlgebra.Parser.Syntax {
    public class ArbitrarySyntaxItem : SyntaxItem {
        public override string Value { get; }

        public ArbitrarySyntaxItem(string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString() {
            return $"'{Value}'";
        }
        
        public override bool Equals(SyntaxItem? other) {
            return other is not null;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value);
        }
    }
}