using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Syntax.Identifiers {
    public static class IdentifierUtils {
        public static IEnumerable<ISyntaxIdentifier> GetSyntaxIdentifiers() {
            return Array.Empty<ISyntaxIdentifier>();
        }

        public static LexemeIdentifier GetLexemeIdentifierFromString(string lexemeValue) {
            return GetLexemeIdentifiers().FirstOrDefault(lexemeIdentifier => lexemeIdentifier.Regex.IsMatch(lexemeValue)) ?? LEXEME_UNKNOWN;
        }

        public static IEnumerable<LexemeIdentifier> GetLexemeIdentifiers() {
            return GetSyntaxIdentifiers().SelectMany(syntaxIdentifier => syntaxIdentifier.GetLexemeIdentifiers()).ToList();
        }

        public static uint GetMaximumSyntaxIdentifierPrecedence() {
            return GetSyntaxIdentifiers().DefaultIfEmpty(SYNTAX_UNKNOWN).Max(syntaxIdentifier => syntaxIdentifier.Precedence);
        }

        public static readonly LexemeIdentifier LEXEME_UNKNOWN = new("UNKNOWN", "^.+$", true);
        public static readonly SingleSyntaxIdentifier SYNTAX_UNKNOWN = new(LEXEME_UNKNOWN, SyntaxIdentifierType.UNKNOWN, 0);
    }
}