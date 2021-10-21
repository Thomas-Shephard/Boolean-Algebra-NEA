using System;
using BooleanAlgebra.Identifiers;

namespace BooleanAlgebra.Lexer.Lexemes {
    /// <summary>
    /// Holds the value of a contextualLexeme.
    /// </summary>
    public class ContextualLexeme : Lexeme, IEquatable<ContextualLexeme> {
        /// <summary>
        /// The string representation of a contextualLexeme.
        /// </summary>
        public string LexemeValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualLexeme"/> class with a <paramref name="lexemeIdentifier"/>, <paramref name="lexemePosition"/> and <paramref name="lexemeValue"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> of the new <see cref="ContextualLexeme"/>.</param>
        /// <param name="lexemePosition">The <see cref="LexemePosition"/> of the new <see cref="ContextualLexeme"/>.</param>
        /// <param name="lexemeValue">The string representation of the new <see cref="ContextualLexeme"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lexemeValue"/> is null.</exception>
        public ContextualLexeme(Identifier identifier, LexemePosition lexemePosition, string lexemeValue) : base (identifier, lexemePosition) {
            LexemeValue = lexemeValue ?? throw new ArgumentNullException(nameof(lexemeValue));  //Ensure that the lexemeValue is not null
        }
        
        public override string ToString() {
            return $"[{Identifier}, {LexemeValue}, {LexemePosition}]";    //Outputs in the format [Identifier, LexemeValue, LexemePosition]
        }
        
        public bool Equals(ContextualLexeme? other) {
            return other is not null                                    //Check that the other lexeme is not null
                   && Identifier.Equals(other.Identifier)               //Check that the identifiers are equal
                   && LexemePosition.Equals(other.LexemePosition)       //Check that the lexemePositions are equal
                   && LexemeValue == other.LexemeValue;                 //Check that the lexemeValues are equal
        }

        public override bool Equals(object? obj) {
            return Equals(obj as ContextualLexeme);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Identifier, LexemePosition, LexemeValue);
        }
    }
}