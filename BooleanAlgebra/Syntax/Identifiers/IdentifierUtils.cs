using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Syntax.Identifiers {
    public static class IdentifierUtils {
        public static IEnumerable<ISyntaxIdentifier> GetSyntaxIdentifiers() {
            return new [] {
                (ISyntaxIdentifier)
                new SingleSyntaxIdentifier(new LexemeIdentifier("OR", "^(?i)OR$", false), SyntaxIdentifierType.BINARY_OPERATOR, 1),
                new SingleSyntaxIdentifier(new LexemeIdentifier("AND", "^(?i)AND$", false), SyntaxIdentifierType.BINARY_OPERATOR, 2),
                new SingleSyntaxIdentifier(new LexemeIdentifier("NOT", "^(?i)NOT$", false), SyntaxIdentifierType.UNARY_OPERATOR, 3),
                new MultipleSyntaxIdentifier(new []{ new LexemeIdentifier("LEFT_PAREN", "^\\($", false), new LexemeIdentifier("RIGHT_PAREN", "^\\)$", false)  }, SyntaxIdentifierType.GROUPING_OPERATOR, 4),
                new SingleSyntaxIdentifier(new LexemeIdentifier("VARIABLE", "^[A-Za-z]+$", true), SyntaxIdentifierType.OPERAND, 5),
                new SingleSyntaxIdentifier(new LexemeIdentifier("LITERAL", "^(1|0)$", true), SyntaxIdentifierType.OPERAND, 5),
            };
        }

        public static LexemeIdentifier GetLexemeIdentifierFromString(string lexemeValue) {
            return GetLexemeIdentifiers().FirstOrDefault(lexemeIdentifier => lexemeIdentifier.Regex.IsMatch(lexemeValue)) ?? LEXEME_ERROR;
        }

        public static IEnumerable<LexemeIdentifier> GetLexemeIdentifiers() {
            return GetSyntaxIdentifiers().SelectMany(syntaxIdentifier => syntaxIdentifier.GetLexemeIdentifiers()).ToList();
        }

        public static uint GetMaximumSyntaxIdentifierPrecedence() {
            return GetSyntaxIdentifiers().Max(syntaxIdentifier => syntaxIdentifier.Precedence);
        }

        public static readonly LexemeIdentifier LEXEME_ERROR = new("ERROR", "", true);
        public static readonly SingleSyntaxIdentifier SYNTAX_ERROR = new(LEXEME_ERROR, SyntaxIdentifierType.UNKNOWN, 0);
    }
}