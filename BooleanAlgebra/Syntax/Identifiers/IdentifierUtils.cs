using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BooleanAlgebra.Syntax.Identifiers {
    public static class IdentifierUtils {
        public static IEnumerable<ISyntaxIdentifier> GetSyntaxIdentifiers() {
            return new [] {
                (ISyntaxIdentifier)
                new SingleSyntaxIdentifier(new LexemeIdentifier("OR", "^(?i)OR$", false), SyntaxIdentifierType.BINARY_OPERATOR, 1),
                new SingleSyntaxIdentifier(new LexemeIdentifier("AND", "^(?i)AND$", false), SyntaxIdentifierType.BINARY_OPERATOR, 2),
                new SingleSyntaxIdentifier(new LexemeIdentifier("NOT", "^(?i)NOT$", false), SyntaxIdentifierType.UNARY_OPERATOR, 3),
                new MultipleSyntaxIdentifier(new []{ new LexemeIdentifier("LEFT_PAREN", "^\\($", false), new LexemeIdentifier("RIGHT_PAREN", "^\\)$", false)  }, SyntaxIdentifierType.GROUPING_OPERATOR, 4),
                new SingleSyntaxIdentifier(new LexemeIdentifier("VARIABLE", "^[A-Za-z]+$", true), SyntaxIdentifierType.VARIABLE_OPERAND, 5),
                new SingleSyntaxIdentifier(new LexemeIdentifier("LITERAL", "^(?i)(TRUE|FALSE)$", true), SyntaxIdentifierType.LITERAL_OPERAND, 5),
            };
        }

        public static IEnumerable<LexemeIdentifier> GetLexemeIdentifiers() {
            return GetSyntaxIdentifiers().SelectMany(syntaxIdentifier => syntaxIdentifier.GetLexemeIdentifiers()).ToList();
        }

        public static uint GetMaximumPrecedence() {
            return GetSyntaxIdentifiers().Max(syntaxIdentifier => syntaxIdentifier.Precedence);
        }

        public static readonly LexemeIdentifier LEXEME_ERROR = new("ERROR", "", false);
        public static readonly SingleSyntaxIdentifier SYNTAX_ERROR = new(LEXEME_ERROR, SyntaxIdentifierType.UNKNOWN, 0);
    }
}