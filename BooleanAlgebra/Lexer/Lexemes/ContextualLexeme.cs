using System;
using BooleanAlgebra.Syntax.Identifiers;

namespace BooleanAlgebra.Lexer.Lexemes {
    public class ContextualLexeme : ILexeme {
        public LexemeIdentifier LexemeIdentifier { get; }
        public LexemePosition LexemePosition { get; }
        public string LexemeValue { get; }

        public ContextualLexeme(LexemeIdentifier lexemeIdentifier, LexemePosition lexemePosition, string lexemeValue) {
            LexemeIdentifier = lexemeIdentifier;
            LexemePosition = lexemePosition;
            LexemeValue = lexemeValue;
        }
        
        public override string ToString() {
            return $"[{LexemeIdentifier}, {LexemeValue}, {LexemePosition}]";
        }
        
        public bool Equals(ILexeme? other) {
            return other is ContextualLexeme otherContextualLexeme
                   && LexemeIdentifier.Equals(otherContextualLexeme.LexemeIdentifier)
                   && LexemePosition.Equals(otherContextualLexeme.LexemePosition)
                   && LexemeValue == otherContextualLexeme.LexemeValue;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as ILexeme);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, LexemePosition, LexemeValue);
        }
    }
}