using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Syntax.Operators {
    public class BinaryOperator : SyntaxItem {
        public string LexemeType { get; }
        public IEnumerable<SyntaxItem> SyntaxItems { get; }

        public BinaryOperator(string lexemeType, SyntaxItem syntaxItem1, SyntaxItem syntaxItem2) 
            : this(lexemeType, new[] {syntaxItem1, syntaxItem2}) { }

        public BinaryOperator(string lexemeType, IEnumerable<SyntaxItem> syntaxItems) {
            if (syntaxItems is null)
                throw new ArgumentNullException(nameof(syntaxItems));
            IEnumerable<SyntaxItem> enumerable = syntaxItems.ToArray();
            if (enumerable.Count() < 2) 
                throw new ArgumentException("There must be at least two syntax items");
            
            LexemeType = lexemeType;
            SyntaxItems = enumerable;
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