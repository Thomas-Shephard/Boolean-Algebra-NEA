using System;
using BooleanAlgebra.Lexer.Lexemes;

namespace BooleanAlgebra.Parser {
    public class ParserException : Exception {
        public LexemePosition LexemePosition { get; }
        
        public ParserException(LexemePosition lexemePosition, string message) : base (message) {
            if (message is null) throw new ArgumentNullException(nameof(message));
            LexemePosition = lexemePosition;
        }
    }
}