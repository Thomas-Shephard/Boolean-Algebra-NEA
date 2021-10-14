using System;
using BooleanAlgebra.Identifiers;

namespace BooleanAlgebra.Lexer.Lexemes {
    /// <summary>
    /// Holds the identifier and position of a lexeme.
    /// </summary>
    public class Lexeme : IEquatable<Lexeme> {
        /// <summary>
        /// The identifier of a lexeme.
        /// </summary>
        public LexemeIdentifier LexemeIdentifier { get; }
        /// <summary>
        /// The position of a lexeme.
        /// </summary>
        public LexemePosition LexemePosition { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualLexeme"/> class with a <paramref name="lexemeIdentifier"/> and <paramref name="lexemePosition"/>.
        /// </summary>
        /// <param name="lexemeIdentifier">The <see cref="LexemeIdentifier"/> of the new <see cref="Lexeme"/>.</param>
        /// <param name="lexemePosition">The <see cref="LexemePosition"/> of the new <see cref="Lexeme"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemeIdentifier"/> or <paramref name="lexemePosition"/> is null.</exception>
        public Lexeme(LexemeIdentifier lexemeIdentifier, LexemePosition lexemePosition) {
            LexemeIdentifier = lexemeIdentifier ?? throw new ArgumentNullException(nameof(lexemeIdentifier));   //Ensure that the lexemeIdentifier is not null
            LexemePosition = lexemePosition ?? throw new ArgumentNullException(nameof(lexemePosition));         //Ensure that the lexemePosition is not null
        }

        public override string ToString() {
            return $"[{LexemeIdentifier}, {LexemePosition}]";   //Outputs in the format [LexemeIdentifier, LexemePosition]
        }

        public bool Equals(Lexeme? other) {
            return other is not null                                    //Check that the other lexeme is not null
                   && LexemeIdentifier.Equals(other.LexemeIdentifier)   //Check that the lexemeIdentifiers are equal
                   && LexemePosition.Equals(other.LexemePosition);      //Check that the lexemePositions are equal
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Lexeme);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, LexemePosition);
        }
    }
}