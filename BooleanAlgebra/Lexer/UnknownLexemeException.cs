namespace BooleanAlgebra.Lexer;
/// <summary>
/// Holds the exception, thrown by the lexer when it experiences an unknown lexeme.
/// </summary>
public sealed class UnknownLexemeException : Exception {
    /// <summary>
    /// The message that describes the error.
    /// </summary>
    public override string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownLexemeException"/> class with a <paramref name="lexemePosition"/> and <paramref name="booleanExpression"/>.
    /// </summary>
    /// <param name="lexemePosition">The position of the error within the input boolean expression.</param>
    /// <param name="booleanExpression">The input boolean expression.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemePosition"/> or <paramref name="booleanExpression"/> is null.</exception>
    public UnknownLexemeException(LexemePosition lexemePosition,  string booleanExpression) {
        if (lexemePosition is null) throw new ArgumentNullException(nameof(lexemePosition));        //Ensure that the lexeme position is not null.
        if (booleanExpression is null) throw new ArgumentNullException(nameof(booleanExpression));  //Ensure that the boolean expression is not null.

        string unknownToken = booleanExpression.Substring(lexemePosition.StartPosition, lexemePosition.Length); //Get the unknown token from the boolean expression.
        Message = $"The symbol '{unknownToken}' was not recognised by the lexer";   //Form a message from the unknown token.
    }
}