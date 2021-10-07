using System;
using System.Collections.Generic;

namespace BooleanAlgebra.Syntax.Identifiers {
    public class SingleSyntaxIdentifier : ISyntaxIdentifier {
        public LexemeIdentifier LexemeIdentifier { get; }
        public SyntaxIdentifierType SyntaxIdentifierType { get; }
        public uint Precedence { get; }

        public SingleSyntaxIdentifier(LexemeIdentifier lexemeIdentifier, SyntaxIdentifierType syntaxIdentifierType, uint precedence) {
            LexemeIdentifier = lexemeIdentifier;
            SyntaxIdentifierType = syntaxIdentifierType;
            Precedence = precedence;
        }
        
        public IEnumerable<LexemeIdentifier> GetLexemeIdentifiers() {
            return new [] { LexemeIdentifier };
        }

        public bool Equals(ISyntaxIdentifier? other) {
            return other is SingleSyntaxIdentifier otherSingleSyntaxIdentifier
                   && LexemeIdentifier.Equals(otherSingleSyntaxIdentifier.LexemeIdentifier)
                   && SyntaxIdentifierType == otherSingleSyntaxIdentifier.SyntaxIdentifierType
                   && Precedence == otherSingleSyntaxIdentifier.Precedence;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as ISyntaxIdentifier);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, SyntaxIdentifierType, Precedence);
        }
    }
}