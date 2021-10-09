using System;
using BooleanAlgebra.Identifiers;

namespace BooleanAlgebra.Lexer.Lexemes {
    public class ContextualLexeme : Lexeme, IEquatable<ContextualLexeme> {
        public string LexemeValue { get; }

        public ContextualLexeme(LexemeIdentifier lexemeIdentifier, LexemePosition lexemePosition, string lexemeValue) : base (lexemeIdentifier, lexemePosition) {
            if(LexemePosition.Length != lexemeValue.Length)
                throw new ArgumentException($"The parameter {nameof(lexemePosition)} must be of an equal length to the parameter {nameof(lexemeValue)}.");
            LexemeValue = lexemeValue ?? throw new ArgumentNullException(nameof(lexemeValue));
        }
        
        public override string ToString() {
            return $"[{LexemeIdentifier}, {LexemeValue}, {LexemePosition}]";
        }
        
        public bool Equals(ContextualLexeme? other) {
            return other is not null
                   && LexemeIdentifier.Equals(other.LexemeIdentifier)
                   && LexemePosition.Equals(other.LexemePosition)
                   && LexemeValue == other.LexemeValue;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as ContextualLexeme);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, LexemePosition, LexemeValue);
        }
    }
}