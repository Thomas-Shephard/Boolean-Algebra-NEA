using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BooleanAlgebra.Utils;

namespace BooleanAlgebra.Parser.Syntax {
    public class BinaryOperator : SyntaxItem {
        public override string Value { get; }
        public List<SyntaxItem> SyntaxItems { get; }

        public BinaryOperator(string lexemeType, IEnumerable<SyntaxItem> syntaxItems) {
            if (syntaxItems is null)
                throw new ArgumentNullException(nameof(syntaxItems));
            List<SyntaxItem> enumerable = syntaxItems.ToList();
            if (enumerable.Count < 2) 
                throw new ArgumentException("There must be at least two syntax items");
            Value = lexemeType ?? throw new ArgumentNullException(nameof(lexemeType));
            SyntaxItems = enumerable.ToList();

                for (int i = SyntaxItems.Count - 1; i >= 0; i--) {
                    if (SyntaxItems[i] is not BinaryOperator binaryOperator ||
                        binaryOperator.Value != lexemeType) continue;
                    SyntaxItems.RemoveAt(i);
                    binaryOperator.SyntaxItems.ForEach(syntaxItem => SyntaxItems.Add(syntaxItem));
                }

        }

        public override string ToString() {
            StringBuilder stringBuilder = new("(");
            for (int i = 0; i < SyntaxItems.Count; i++) {
                stringBuilder.Append($" {SyntaxItems[i]}");
                if (i < SyntaxItems.Count - 1) {
                    stringBuilder.Append($" {Value}");
                }
            }
            return stringBuilder.Append(')').ToString();
        }

        public override bool Equals(SyntaxItem? other) {
            return other is BinaryOperator otherBinaryOperator
                   && Value == otherBinaryOperator.Value
                   && SyntaxItems.SequenceEqualsIgnoreOrder(otherBinaryOperator.SyntaxItems);
        }

        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value, SyntaxItems);
        }
        
        
    }
}