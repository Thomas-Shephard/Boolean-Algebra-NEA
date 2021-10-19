using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Identifiers {
    public static class IdentifierUtils {
        public static IEnumerable<ISyntaxIdentifier> GetSyntaxIdentifiers() {
            return new [] {
                (ISyntaxIdentifier)
                new SingleSyntaxIdentifier(new LexemeIdentifier("OR", "^((?i)OR|\\|)$", false), SyntaxIdentifierType.BINARY_OPERATOR, 1),
                new SingleSyntaxIdentifier(new LexemeIdentifier("AND", "^((?i)AND|&)$", false), SyntaxIdentifierType.BINARY_OPERATOR, 2),
                new SingleSyntaxIdentifier(new LexemeIdentifier("NOT", "^((?i)NOT|!)$", false), SyntaxIdentifierType.UNARY_OPERATOR, 3),
                new MultipleSyntaxIdentifier(new []{ new LexemeIdentifier("LEFT_PAREN", "^\\($", false), new LexemeIdentifier("RIGHT_PAREN", "^\\)$", false)  }, SyntaxIdentifierType.GROUPING_OPERATOR, 4),
                new SingleSyntaxIdentifier(new LexemeIdentifier("VARIABLE", "^[A-Za-z]+$", true), SyntaxIdentifierType.OPERAND, 5),
                new SingleSyntaxIdentifier(new LexemeIdentifier("LITERAL", "^(1|0)$", true), SyntaxIdentifierType.OPERAND, 5),
            };
            //return Array.Empty<ISyntaxIdentifier>();
        }

        public static bool TryGetLexemeIdentifierFromString(string lexemeValue, out LexemeIdentifier lexemeIdentifier) {
            LexemeIdentifier? tempLexemeIdentifier = GetLexemeIdentifiers().FirstOrDefault(identifier => identifier.Regex.IsMatch(lexemeValue));
            if (tempLexemeIdentifier is not null) {
                lexemeIdentifier = tempLexemeIdentifier;
                return true;
            }
                
            lexemeIdentifier = LEXEME_UNKNOWN;
            return false;
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