using System;
using System.Collections.Generic;
using System.Linq;

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
            return SyntaxItems.Aggregate("(", (current, operand) => current + operand + " " + Value + " ")[..^(Value.Length + 2)] + ")";
        }

        public override bool Equals(SyntaxItem? other) {
            return other is BinaryOperator otherBinaryOperator
                   && Value == otherBinaryOperator.Value
                   && ScrambledEquals(SyntaxItems, otherBinaryOperator.SyntaxItems);
        }

        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value, SyntaxItems);
        }
        
        public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2) {
            Dictionary<T, int>? cnt = new();
            foreach (T s in list1) {
                if (cnt.ContainsKey(s)) {
                    cnt[s]++;
                } else {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2) {
                if (cnt.ContainsKey(s)) {
                    cnt[s]--;
                } else {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}