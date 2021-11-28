namespace BooleanAlgebra.Identifiers;
public static class IdentifierUtils {
    private static Identifier[]? _identifiers;

    private static IEnumerable<Identifier> GetIdentifiers() {
        return _identifiers ??= GenerateIdentifiers();
    }
    
    private static Identifier[] GenerateIdentifiers() {
        return new[] {
            new Identifier(IdentifierType.BINARY_OPERATOR, 1, "+", "^((?i)OR|\\+)$", false, false),
            new Identifier(IdentifierType.BINARY_OPERATOR, 2, ".", "^((?i)AND|\\.)$", false, false),
            new Identifier(IdentifierType.UNARY_OPERATOR, 3, "!", "^((?i)NOT|\\!)$", false, false),
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "PARENTHESIS", "^\\($", false, false),
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "PARENTHESIS", "^\\)$", false, false),
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "REPEATING", "^\\[$", false, true),
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "REPEATING", "^\\]$", false, true),
            new Identifier(IdentifierType.OPERAND, 5, "VARIABLE", "^[A-Za-z]+$", true, false),
            new Identifier(IdentifierType.OPERAND, 5, "LITERAL", "^(1|0)$", true, false)
        };
    }
    
    public static bool TryGetIdentifierFromLexemeValue(string lexemeValue, bool useGenericOperands, [NotNullWhen(true)] out Identifier? matchedIdentifier) {
        return GetIdentifiers().TryFirst(identifier => {
            if (!identifier.Regex.IsMatch(lexemeValue))
                return false;
            return useGenericOperands || !identifier.IsGenericOperand;
        }, out matchedIdentifier);
    }

    public static int GetMaximumPrecedence() {
        return GetIdentifiers().Max(syntaxIdentifier => syntaxIdentifier.Precedence);
    }
}