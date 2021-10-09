using System;

namespace BooleanAlgebra.Parser.Syntax {
    public class GroupingOperator : SyntaxItem {
        public SyntaxItem SyntaxItem { get; }

        public GroupingOperator(SyntaxItem syntaxItem) {
            SyntaxItem = syntaxItem;
        }

        public override string ToString() {
            return SyntaxItem.ToString();
        }
        
        public override bool Equals(SyntaxItem? other) {
            return other is GroupingOperator otherGroupingOperator
                   && SyntaxItem.Equals(otherGroupingOperator.SyntaxItem);
        }

        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine("GroupingOperator", SyntaxItem);
        }
    }
}