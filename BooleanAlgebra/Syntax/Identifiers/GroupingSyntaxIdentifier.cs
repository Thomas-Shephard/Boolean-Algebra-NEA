using System;
using BooleanAlgebra.Lexer.Lexemes;

namespace BooleanAlgebra.Syntax.Identifiers {
    public class GroupingSyntaxIdentifier : SyntaxIdentifier {
        private LexemeIdentifier LexemeIdentifier2 { get; }
        
        public GroupingSyntaxIdentifier(LexemeIdentifier lexemeIdentifier1, LexemeIdentifier lexemeIdentifier2, SyntaxIdentifierType syntaxIdentifierType,
            uint precedence) : base(lexemeIdentifier1, syntaxIdentifierType, precedence) {
            LexemeIdentifier2 = lexemeIdentifier2;
        }
        
        public bool Equals(SyntaxIdentifier? other) {
            return other is GroupingSyntaxIdentifier otherGroupingSyntaxIdentifier
                   && base.Equals(other)
                   && LexemeIdentifier2.Equals(otherGroupingSyntaxIdentifier.LexemeIdentifier2);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as SyntaxIdentifier);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), LexemeIdentifier2);
        }
    }
}