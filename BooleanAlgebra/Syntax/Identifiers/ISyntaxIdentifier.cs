using System;
using System.Collections.Generic;

namespace BooleanAlgebra.Syntax.Identifiers {
    public interface ISyntaxIdentifier : IEquatable<ISyntaxIdentifier> {
        public SyntaxIdentifierType SyntaxIdentifierType { get; }
        public uint Precedence { get; }
        public IEnumerable<LexemeIdentifier> GetLexemeIdentifiers();
    }
}