using System;

namespace BooleanAlgebra.Parser.Syntax {
    /// <summary>
    /// 
    /// </summary>
    public class UnaryOperator : SyntaxItem {
        /// <summary>
        /// 
        /// </summary>
        public override string Value { get; }
        /// <summary>
        /// 
        /// </summary>
        public SyntaxItem SyntaxItem { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lexemeType"></param>
        /// <param name="syntaxItem"></param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemeType"/> or <paramref name="syntaxItem"/> is null.</exception>
        public UnaryOperator(string lexemeType, SyntaxItem syntaxItem) {
            Value = lexemeType ?? throw new ArgumentNullException(nameof(lexemeType));      //Ensure that the lexemeType is not null
            SyntaxItem = syntaxItem ?? throw new ArgumentNullException(nameof(syntaxItem)); //Ensure that the syntaxItem is not null
        }

        public override string ToString() {
            return $"[{Value}, {SyntaxItem}]";  //Outputs in the format [Value, SyntaxItem]
        }

        public override bool Equals(SyntaxItem? other) {
            return other is UnaryOperator otherUnaryOperator            //Check that the other syntaxItem is of the same type
                   && Value == otherUnaryOperator.Value                 //Check that the values are equal
                   && SyntaxItem.Equals(otherUnaryOperator.SyntaxItem); //Check that the daughter syntaxItems are equal
        }
        
        public override bool Equals(object? other) {
            return Equals(other as SyntaxItem);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Value, SyntaxItem);
        }
    }
}