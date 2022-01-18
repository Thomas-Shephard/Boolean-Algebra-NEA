﻿namespace BooleanAlgebra.Identifiers;
/// <summary>
/// Provides a set of methods to interact with the possible identifiers.
/// </summary>
public static class IdentifierUtils {
    /// <summary>
    /// Stores a cache of the possible identifiers that can be matched from a given lexeme value.
    /// </summary>
    private static Identifier[]? _identifiers;

    /// <summary>
    /// Provides a collection of the possible identifiers that can be matched from a given lexeme value.
    /// </summary>
    /// <returns>A collection of the possible identifiers that can be matched from a given lexeme value.</returns>
    private static IEnumerable<Identifier> GetIdentifiers() {
        //If the identifiers array has not been initialized, do so before returning it.
        return _identifiers ??= GenerateIdentifiers();
    }
    
    /// <summary>
    /// Provides an array of all the possible identifiers that can be matched from a given lexeme value.
    /// </summary>
    /// <returns>An array of possible identifiers that can be matched from a given lexeme value.</returns>
    private static Identifier[] GenerateIdentifiers() {
        return new[] {
            // This is an OR operator, it matches 'OR' (case insensitive) and '+'. It has a precedence of 1 and doesn't require any context.
            new Identifier(IdentifierType.BINARY_OPERATOR, 1, "+", "^((?i)OR|\\+)$", false, false),
            //This is an AND operator, it matches 'AND' (case insensitive) and '.'. It has a precedence of 2 and doesn't require any context.
            new Identifier(IdentifierType.BINARY_OPERATOR, 2, ".", "^((?i)AND|\\.)$", false, false),
            //This is a NOT operator, it matches 'NOT' (case insensitive) and '!'. It has a precedence of 3 and doesn't require any context.
            new Identifier(IdentifierType.UNARY_OPERATOR, 3, "!", "^((?i)NOT|\\!)$", false, false),
            //This is a LEFT PARENTHESIS operator, it matches '('. It has a precedence of 4 and doesn't require any context.
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "PARENTHESIS", "^\\($", false, false),
            //This is a RIGHT PARENTHESIS operator, it matches ')'. It has a precedence of 4 and doesn't require any context.
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "PARENTHESIS", "^\\)$", false, false),
            //This is a LEFT SQUARE BRACKET operator, it matches '['. It has a precedence of 4 and doesn't require any context.
            //This identifier can only be used when the program is assembling syntax trees for the simplification algorithm.
            new Identifier(IdentifierType.GROUPING_OPERATOR_START, 4, "REPEATING", "^\\[$", false, true),
            //This is a RIGHT SQUARE BRACKET operator, it matches ']'. It has a precedence of 4 and doesn't require any context.
            //This identifier can only be used when the program is assembling syntax trees for the simplification algorithm.
            new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "REPEATING", "^\\]$", false, true),
            //This is a variable operand, it matches one or more letters. It has a precedence of 5 and requires context.
            new Identifier(IdentifierType.OPERAND, 5, "VARIABLE", "^[A-Za-z]+$", true, false),
            //This is a boolean operand, it matches '0' or '1'. It has a precedence of 5 and requires context.
            new Identifier(IdentifierType.OPERAND, 5, "LITERAL", "^(1|0)$", true, false)
        };
    }
    
    /// <summary>
    /// Indicates whether a specified lexeme value has a matching identifier.
    /// Provides the matched identifier if it does.
    /// </summary>
    /// <param name="lexemeValue">The lexeme value that an identifier is found from.</param>
    /// <param name="useGenericOperands">Specifies whether or not the lexer is using generic operands.</param>
    /// <param name="matchedIdentifier">The first identifier that has a regular expression that matches the lexeme value.</param>
    /// <returns>True when an identifier is found that matches the lexeme value.</returns>
    public static bool TryGetIdentifierFromLexemeValue(string lexemeValue, bool isBuildingSimplificationRule, [NotNullWhen(true)] out Identifier? matchedIdentifier) {
        //Try to find an identifier that matches the lexeme value.
        return GetIdentifiers().TryFirst(identifier => {
            //If the lexeme value does not match the identifier's regex then it cannot be a match.
            if (!identifier.Regex.IsMatch(lexemeValue))
                return false;
            //If the identifier is a generic operand and generic operands are not being used then it cannot be a match.
            return isBuildingSimplificationRule || !identifier.IsSimplificationRuleSpecific;
        }, out matchedIdentifier);
    }

    /// <summary>
    /// Returns the maximum precedence from the available identifiers.
    /// </summary>
    /// <returns>The maximum precedence from the available identifiers.</returns>
    public static int GetMaximumPrecedence() {
        return GetIdentifiers().Max(syntaxIdentifier => syntaxIdentifier.Precedence);
    }
}