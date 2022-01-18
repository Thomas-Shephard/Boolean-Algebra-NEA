namespace BooleanAlgebra.Identifiers;
/// <summary>
/// Provides a set of methods to interact with the possible identifiers.
/// </summary>
public static class IdentifierUtils {
    /// <summary>
    /// Stores a cache of all the possible identifiers.
    /// </summary>
    private static Identifier[]? _identifiers;

    /// <summary>
    /// Provides a collection of all the possible identifiers.
    /// </summary>
    /// <returns>A collection of all the possible identifiers.</returns>
    private static IEnumerable<Identifier> GetIdentifiers() {
        //If the cache is empty, generate the identifiers first.
        return _identifiers ??= GenerateIdentifiers();
    }
    
    /// <summary>
    /// Generates a new collection of all the possible identifiers.
    /// </summary>
    /// <returns>A new collection of all the possible identifiers.</returns>
    private static Identifier[] GenerateIdentifiers() {
        return new[] {
            //This is an OR operator, it matches 'OR' (case insensitive) and '+'.
            new Identifier(IdentifierType.BINARY_OPERATOR, 1, "+", "^((?i)OR|\\+)$", false, false),
            //This is an AND operator, it matches 'AND' (case insensitive) and '.'.
            new Identifier(IdentifierType.BINARY_OPERATOR, 2, ".", "^((?i)AND|\\.)$", false, false),
            //This is a NOT operator, it matches 'NOT' (case insensitive) and '!'.
            new Identifier(IdentifierType.UNARY_OPERATOR, 3, "!", "^((?i)NOT|\\!)$", false, false),
            //This is a LEFT PARENTHESIS operator, it matches '('.
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "PARENTHESIS", "^\\($", false, false),
            //This is a RIGHT PARENTHESIS operator, it matches ')'.
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "PARENTHESIS", "^\\)$", false, false),
            //This is a LEFT SQUARE BRACKET operator, it matches '['.
            //This identifier can only be used when the program is assembling syntax trees for the simplification algorithm.
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "REPEATING", "^\\[$", false, true),
            //This is a RIGHT SQUARE BRACKET operator, it matches ']'.
            //This identifier can only be used when the program is assembling syntax trees for the simplification algorithm.
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "REPEATING", "^\\]$", false, true),
            //This is a variable operand, it matches one or more letters.
            new Identifier(IdentifierType.OPERAND, 5, "VARIABLE", "^[A-Za-z]+$", true, false),
            //This is a boolean operand, it matches '0' or '1'.
            new Identifier(IdentifierType.OPERAND, 5, "LITERAL", "^(1|0)$", true, false)
        };
    }
    
    /// <summary>
    /// Indicates whether a specified lexeme value has a matching identifier.
    /// Provides the matched identifier if it does.
    /// </summary>
    /// <param name="symbol">The text symbol that an identifier should be found from.</param>
    /// <param name="isBuildingSimplificationRule">Determines whether or not it should also match for identifiers only used in the simplification rule builder.</param>
    /// <param name="matchedIdentifier">The first identifier that has a regular expression that matches the lexeme value, null if no identifier is found.</param>
    /// <returns>True when an identifier is found that matches the lexeme value.</returns>
    public static bool TryGetIdentifierFromSymbol(string symbol, bool isBuildingSimplificationRule, [NotNullWhen(true)] out Identifier? matchedIdentifier) {
        //Try to find an identifier that matches the lexeme value.
        return GetIdentifiers().TryFirst(identifier => {
            //If the symbol does not match the identifier's regex then it cannot be a match.
            if (!identifier.Regex.IsMatch(symbol))
                return false;
            //If the identifier can only be used while building simplification rules and a simplification rule is not being built then it cannot be a match.
            return isBuildingSimplificationRule || !identifier.IsSimplificationRuleSpecific;
        }, out matchedIdentifier);
    }

    /// <summary>
    /// Returns the maximum precedence from all of the possible identifiers.
    /// </summary>
    /// <returns>The maximum precedence from all of the possible identifiers.</returns>
    public static int GetMaximumPrecedence() {
        return GetIdentifiers().Max(syntaxIdentifier => syntaxIdentifier.Precedence);
    }
}