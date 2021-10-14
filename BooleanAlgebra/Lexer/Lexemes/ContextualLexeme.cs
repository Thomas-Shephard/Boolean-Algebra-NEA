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
        /// <param name="lexemeIdentifier">The <see cref="LexemeIdentifier"/> of the new <see cref="ContextualLexeme"/>.</param>
        /// <param name="lexemePosition">The <see cref="LexemePosition"/> of the new <see cref="ContextualLexeme"/>.</param>
        /// <param name="lexemeValue">The string representation of the new <see cref="ContextualLexeme"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lexemeValue"/> is null.</exception>
        public ContextualLexeme(LexemeIdentifier lexemeIdentifier, LexemePosition lexemePosition, string lexemeValue) : base (lexemeIdentifier, lexemePosition) {
            LexemeValue = lexemeValue ?? throw new ArgumentNullException(nameof(lexemeValue));  //Ensure that the lexemeValue is not null
        }
        
        public override string ToString() {
            return $"[{LexemeIdentifier}, {LexemeValue}, {LexemePosition}]";    //Outputs in the format [LexemeIdentifier, LexemeValue, LexemePosition]
        }
        
        public bool Equals(ContextualLexeme? other) {
            return other is not null                                    //Check that the other lexeme is not null
                   && LexemeIdentifier.Equals(other.LexemeIdentifier)   //Check that the lexemeIdentifiers are equal
                   && LexemePosition.Equals(other.LexemePosition)       //Check that the lexemePositions are equal
                   && LexemeValue == other.LexemeValue;                 //Check that the lexemeValues are equal
        }

        public override bool Equals(object? obj) {
            return Equals(obj as ContextualLexeme);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, LexemePosition, LexemeValue);
        }
    }
}