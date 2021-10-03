using System;

namespace BooleanAlgebra.Lexer.Lexemes {
    public interface ILexeme : IEquatable<ILexeme> {
        public LexemeIdentifier LexemeIdentifier { get; }
        public LexemePosition LexemePosition { get; }
    }
}