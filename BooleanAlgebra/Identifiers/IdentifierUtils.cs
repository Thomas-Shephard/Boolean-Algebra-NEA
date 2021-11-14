namespace BooleanAlgebra.Identifiers;
public static class IdentifierUtils {
    public static readonly Identifier UnknownIdentifier = new(IdentifierType.UNKNOWN, 0, "UNKNOWN", "^.+$", true);
    
    private static IEnumerable<Identifier> GetIdentifiers() {
        return new[] {
            new Identifier(IdentifierType.BINARY_OPERATOR, 1, "OR", "^((?i)OR|\\||\\+)$", false),
            new Identifier(IdentifierType.BINARY_OPERATOR, 2, "AND", "^((?i)AND|\\&|\\.)$", false),
            new Identifier(IdentifierType.UNARY_OPERATOR, 3, "NOT", "^((?i)NOT|\\!)$", false),
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "PARENTHESIS", "^\\($", false),
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "PARENTHESIS", "^\\)$", false),
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "REPEATING", "^\\[$", false),
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "REPEATING", "^\\]$", false),
            new Identifier(IdentifierType.OPERAND, 5, "VARIABLE", "^[A-Za-z]+$", true),
            new Identifier(IdentifierType.OPERAND, 5, "LITERAL", "^(1|0)$", true)
        };
    }

    public static Identifier GetIdentifierFromLexemeValue(string lexemeValue) {
        return GetIdentifiers().FirstOrDefault(identifier => identifier.Regex.IsMatch(lexemeValue)) ?? UnknownIdentifier;
    }

    public static uint GetMaximumPrecedence() {
        return GetIdentifiers().DefaultIfEmpty(UnknownIdentifier).Max(syntaxIdentifier => syntaxIdentifier.Precedence);
    }
}