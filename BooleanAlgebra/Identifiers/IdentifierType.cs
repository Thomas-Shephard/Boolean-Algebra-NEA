namespace BooleanAlgebra.Identifiers;
/// <summary>
/// Provides the different types that an identifier can be.
/// </summary>
public enum IdentifierType {
    //A binary operator is an operator that has two or more daughter items.
    //For example, in the expression 'A OR B OR C', 'OR' is the binary operator.
    BINARY_OPERATOR,
    //A unary operator is an operator that has one daughter item.
    //For example, in the expression 'NOT A', 'NOT' is the unary operator.
    UNARY_OPERATOR,
    //A grouping operator start is the opening bracket of a group.
    //It is used to change the precedence of the group.
    //For example, in the expression '(A OR B) AND C', '(' is the grouping operator start.
    GROUPING_OPERATOR_START,
    //A grouping operator end is the closing bracket of a group.
    //It is used to change the precedence of the group.
    //For example, in the expression '(A OR B) AND C', ')' is the grouping operator end.
    GROUPING_OPERATOR_END,
    //An operand is a variable or a constant. It is a leaf node and has no daughter items.
    //For example, in the expression 'NOT A', 'A' is the operand
    OPERAND,
}