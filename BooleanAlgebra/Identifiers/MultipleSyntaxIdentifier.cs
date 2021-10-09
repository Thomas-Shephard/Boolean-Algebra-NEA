using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Identifiers {
    public class MultipleSyntaxIdentifier : ISyntaxIdentifier {
        public LexemeIdentifier[] LexemeIdentifiers { get; }
        public SyntaxIdentifierType SyntaxIdentifierType { get; }
        public uint Precedence { get; }
        
        public MultipleSyntaxIdentifier(LexemeIdentifier[] lexemeIdentifiers, SyntaxIdentifierType syntaxIdentifierType, uint precedence) {
            LexemeIdentifiers = lexemeIdentifiers;
            SyntaxIdentifierType = syntaxIdentifierType;
            Precedence = precedence;
        }
        
        public IEnumerable<LexemeIdentifier> GetLexemeIdentifiers() {
            return LexemeIdentifiers;
        }

        public bool Equals(ISyntaxIdentifier? other) {
            return other is MultipleSyntaxIdentifier otherMultipleSyntaxIdentifier
                   && LexemeIdentifiers.SequenceEqual(otherMultipleSyntaxIdentifier.LexemeIdentifiers)
                   && SyntaxIdentifierType == otherMultipleSyntaxIdentifier.SyntaxIdentifierType
                   && Precedence == otherMultipleSyntaxIdentifier.Precedence;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as ISyntaxIdentifier);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifiers, SyntaxIdentifierType, Precedence);
        }
    }
}