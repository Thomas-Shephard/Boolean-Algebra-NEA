using System;

namespace BooleanAlgebra.Parser.Syntax {
    public class UnaryOperator : SyntaxItem {
        public string LexemeType { get; }
        public SyntaxItem SyntaxItem { get; }

        public UnaryOperator(string lexemeType, SyntaxItem syntaxItem) {
            LexemeType = lexemeType;
            SyntaxItem = syntaxItem;
        }

        public override string ToString() {
            return $"{LexemeType} {SyntaxItem}";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is UnaryOperator otherUnaryOperator
                   && LexemeType == otherUnaryOperator.LexemeType
                   && SyntaxItem.Equals(otherUnaryOperator.SyntaxItem);
        }
        
        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeType, SyntaxItem);
        }
    }
}