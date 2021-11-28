namespace BooleanAlgebra.Identifiers;
/// <summary>
/// Provides the different types that an identifier can be.
/// </summary>
public enum IdentifierType {
    //A binary operator is an operator that has two or more daughter items.
    //e.g. 'A OR B OR C' in this case 'OR' is the binary operator
    BINARY_OPERATOR,
    //A unary operator is an operator that has one daughter item.
    //e.g. 'NOT A' in this case 'NOT' is the unary operator
    UNARY_OPERATOR,
    //A grouping operator start is the opening bracket of a group.
    //It is used to change the precedence of the group.
    //e.g. '(A OR B) AND C' in this case '(' is the grouping operator start
    GROUPING_OPERATOR_START,
    //A grouping operator end is the closing bracket of a group.
    //It is used to change the precedence of the group.
    //e.g. '(A OR B) AND C' in this case ')' is the grouping operator end
    GROUPING_OPERATOR_END,
    //An operand is a variable or a constant. It is a leaf node and has no daughter items.
    //e.g. 'A' in this case 'A' is the operand
    OPERAND,
}