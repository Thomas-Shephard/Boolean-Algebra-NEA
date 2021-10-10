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
            IEnumerable<SyntaxItem> enumerable = syntaxItems.ToArray();
            if (enumerable.Count() < 2) 
                throw new ArgumentException("There must be at least two syntax items");
            
            Value = lexemeType;
            SyntaxItems = enumerable.ToList();
        }

        public IEnumerable<BinaryOperator> GetConstituentBinaryOperators(int startPosition = 0) {
            List<BinaryOperator> returnValue = new();
            for (int currentPosition = startPosition + 1; currentPosition < SyntaxItems.Count; currentPosition++) {
                returnValue.Add(new BinaryOperator(Value, SyntaxItems[startPosition], SyntaxItems[currentPosition]));   
            }

            if (startPosition + 1 < SyntaxItems.Count)
                returnValue = returnValue.Concat(GetConstituentBinaryOperators(startPosition + 1)).ToList();
            return returnValue;
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