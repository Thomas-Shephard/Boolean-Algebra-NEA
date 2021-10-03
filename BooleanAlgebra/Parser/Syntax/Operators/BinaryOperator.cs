using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Parser.Syntax.Operators {
    public class BinaryOperator : SyntaxItem {
        public string LexemeType { get; }
        public IEnumerable<SyntaxItem> SyntaxItems { get; }

        public BinaryOperator(string lexemeType, IEnumerable<SyntaxItem> syntaxItems) {
            LexemeType = lexemeType;
            SyntaxItems = syntaxItems;
        }

        public override string ToString() {
            return SyntaxItems.Aggregate("(", (current, operand) => current + operand + " " + LexemeType + " ")[..^(LexemeType.ToString().Length + 2)] + ")";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is BinaryOperator otherBinaryOperator
                   && LexemeType == otherBinaryOperator.LexemeType
                   && SyntaxItems.SequenceEqual(otherBinaryOperator.SyntaxItems);
        }

        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeType, SyntaxItems);
        }
    }
}