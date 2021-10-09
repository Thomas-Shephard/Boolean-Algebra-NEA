using System;
using BooleanAlgebra.Syntax.Identifiers;

namespace BooleanAlgebra.Lexer.Lexemes {
    public class Lexeme : IEquatable<Lexeme> {
        public LexemeIdentifier LexemeIdentifier { get; }
        public LexemePosition LexemePosition { get; }
        
        public Lexeme(LexemeIdentifier lexemeIdentifier, LexemePosition lexemePosition) {
            LexemeIdentifier = lexemeIdentifier;
            LexemePosition = lexemePosition;
        }

        public override string ToString() {
            return $"[{LexemeIdentifier}, {LexemePosition}]";
        }

        public bool Equals(Lexeme? other) {
            return other is not null
                   && LexemeIdentifier.Equals(other.LexemeIdentifier)
                   && LexemePosition.Equals(other.LexemePosition);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Lexeme);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, LexemePosition);
        }
    }
}