using System;
using BooleanAlgebra.Syntax.Identifiers;

namespace BooleanAlgebra.Lexer.Lexemes {
    public interface ILexeme : IEquatable<ILexeme> {
        public LexemeIdentifier LexemeIdentifier { get; }
        public LexemePosition LexemePosition { get; }
    }
}