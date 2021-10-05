using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Syntax.Identifiers {
    public class SyntaxIdentifier : IEquatable<SyntaxIdentifier> {
        public LexemeIdentifier LexemeIdentifier { get; }
        public SyntaxIdentifierType SyntaxIdentifierType { get; }
        public uint Precedence { get; }

        public SyntaxIdentifier(LexemeIdentifier lexemeIdentifier, SyntaxIdentifierType syntaxIdentifierType,
            uint precedence) {
            LexemeIdentifier = lexemeIdentifier;
            SyntaxIdentifierType = syntaxIdentifierType;
            Precedence = precedence;
        }

        public static IEnumerable<SyntaxIdentifier> GetSyntaxIdentifiers() {
            return new[] {
                new SyntaxIdentifier(LexemeIdentifier.OR, SyntaxIdentifierType.BINARY_OPERATOR, 1),
                new SyntaxIdentifier(LexemeIdentifier.AND, SyntaxIdentifierType.BINARY_OPERATOR, 2),
                new SyntaxIdentifier(LexemeIdentifier.NOT, SyntaxIdentifierType.UNARY_OPERATOR, 3),
                new GroupingSyntaxIdentifier(LexemeIdentifier.LEFT_PARENTHESIS, LexemeIdentifier.RIGHT_PARENTHESIS,
                    SyntaxIdentifierType.GROUPING_OPERATOR, 4),
                new SyntaxIdentifier(LexemeIdentifier.LITERAL, SyntaxIdentifierType.LITERAL_OPERAND, 5),
                new SyntaxIdentifier(LexemeIdentifier.VARIABLE, SyntaxIdentifierType.VARIABLE_OPERAND, 5),
            };
        }

        public static uint GetMaximumPrecedence() {
            return GetSyntaxIdentifiers().Max(syntaxIdentifier => syntaxIdentifier.Precedence);
        }

        public static readonly SyntaxIdentifier UNKNOWN = new(LexemeIdentifier.UNKNOWN, SyntaxIdentifierType.UNKNOWN, 0);

        public bool Equals(SyntaxIdentifier? other) {
            return other is not null
                   && LexemeIdentifier.Equals(other.LexemeIdentifier)
                   && SyntaxIdentifierType == other.SyntaxIdentifierType
                   && Precedence == other.Precedence;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as SyntaxIdentifier);
        }

        public override int GetHashCode() {
            return HashCode.Combine(LexemeIdentifier, SyntaxIdentifierType, Precedence);
        }
    }
}