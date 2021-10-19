using System;
using System.Linq;

namespace BooleanAlgebra.Parser.Syntax {
    public class UnaryOperator : SyntaxItem {
        public override string Value { get; }
        public SyntaxItem SyntaxItem { get; }

        public UnaryOperator(string lexemeType, SyntaxItem syntaxItem) {
            Value = lexemeType ?? throw new ArgumentNullException(nameof(lexemeType));
            SyntaxItem = syntaxItem ?? throw new ArgumentNullException(nameof(syntaxItem));
        }

        public override string ToString() {
            return $"{Value} {SyntaxItem}";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is UnaryOperator otherUnaryOperator
                   && Value == otherUnaryOperator.Value
                   && SyntaxItem.Equals(otherUnaryOperator.SyntaxItem);
        }
        
        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value, SyntaxItem);
        }
    }
}