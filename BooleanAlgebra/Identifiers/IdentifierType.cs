namespace BooleanAlgebra.Identifiers;
/// <summary>
/// Provides the different types that an identifier can be.
/// </summary>
public enum IdentifierType {
    /// <summary>
    /// A binary operator is an operator that has two or more daughter items.
    /// </summary>
    /// <example>In the expression 'A OR B OR C', 'OR' is the binary operator.</example>
    BINARY_OPERATOR,
    /// <summary>
    /// A unary operator is an operator that has one daughter item.
    /// </summary>
    /// <example>In the expression 'NOT A', 'NOT' is the unary operator.</example>
    UNARY_OPERATOR,
    /// <summary>
    /// A grouping operator start is the opening bracket of a group.
    /// Grouping operators are used to prioritise an encapsulated expression.
    /// </summary>
    /// <example>In the expression '(A OR B) AND C', '(' is the grouping operator start.</example>
    GROUPING_OPERATOR_START,
    /// <summary>
    /// A grouping operator end is the closing bracket of a group.
    /// </summary>
    /// <example>In the expression '(A OR B) AND C', ')' is the grouping operator end.</example>
    GROUPING_OPERATOR_END,
    /// <summary>
    /// An operand is a variable or a constant. It is a leaf node and has no daughter items.
    /// </summary>
    /// <example>In the expression 'NOT A', 'A' is the operand.</example>
    OPERAND
}