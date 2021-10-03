using System;

namespace BooleanAlgebra.Lexer.Lexemes {
    public class ContextFreeLexeme : ILexeme {
        public LexemeIdentifier LexemeIdentifier { get; }
        public LexemePosition LexemePosition { get; }
        
        public ContextFreeLexeme(LexemeIdentifier lexemeIdentifier, LexemePosition lexemePosition) {
            LexemeIdentifier = lexemeIdentifier;
            LexemePosition = lexemePosition;
        }

        public override string ToString() {
            return $"[{LexemeIdentifier}, {LexemePosition}]";
        }

        public bool Equals(ILexeme? other) {
            return other is ContextFreeLexeme otherContextFreeLexeme
                   && LexemeIdentifier.Equals(otherContextFreeLexeme.LexemeIdentifier)
                   && LexemePosition.Equals(otherContextFreeLexeme.LexemePosition);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as ILexeme);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, LexemePosition);
        }
    }
}