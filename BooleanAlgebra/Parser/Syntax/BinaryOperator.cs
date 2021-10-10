using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Parser.Syntax {
    public class BinaryOperator : SyntaxItem {
        public override string Value { get; }
        public List<SyntaxItem> SyntaxItems { get; }

        public BinaryOperator(string lexemeType, SyntaxItem syntaxItem1, SyntaxItem syntaxItem2) 
            : this(lexemeType, new[] {syntaxItem1, syntaxItem2}) { }

        public BinaryOperator(string lexemeType, IEnumerable<SyntaxItem> syntaxItems) {
            if (syntaxItems is null)
                throw new ArgumentNullException(nameof(syntaxItems));
            List<SyntaxItem> enumerable = syntaxItems.ToList();
            if (enumerable.Count < 2) 
                throw new ArgumentException("There must be at least two syntax items");
            Value = lexemeType;
            SyntaxItems = enumerable.ToList();
            
            bool hasFoundMatch;
            do {
                hasFoundMatch = false;
                for (int i = SyntaxItems.Count - 1; i >= 0; i--) {
                    if (SyntaxItems[i] is not BinaryOperator binaryOperator ||
                        binaryOperator.Value != lexemeType) continue;
                    SyntaxItems.RemoveAt(i);
                    binaryOperator.SyntaxItems.ForEach(syntaxItem => SyntaxItems.Add(syntaxItem));
                    hasFoundMatch = true;
                }
            } while (hasFoundMatch);
        }
        
        public override SyntaxItem[] GetAllSyntaxItems() {
            return new[] {this}.Concat(SyntaxItems).ToArray();
        }

        public override SyntaxItem[] GetSyntaxItems() {
            return SyntaxItems.ToArray();
        }

        public override string ToString() {
            return SyntaxItems.Aggregate("(", (current, operand) => current + operand + " " + Value + " ")[..^(Value.Length + 2)] + ")";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is BinaryOperator otherBinaryOperator
                   && Value == otherBinaryOperator.Value
                   && SyntaxItems.SequenceEqual(otherBinaryOperator.SyntaxItems);
        }

        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value, SyntaxItems);
        }
    }
}