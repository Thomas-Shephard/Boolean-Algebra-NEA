namespace BooleanAlgebra.Lexer;
/// <summary>
/// Holds the exception when the lexer experiences a sequence of characters that could not be matched to an identifier.
/// </summary>
public sealed class UnknownLexemeException : Exception {
    /// <summary>
    /// The message that describes the exception.
    /// The message contains the sequence of characters that could not be matched to an identifier.
    /// </summary>
    public override string Message { get; }

    /// <summary>
    /// Instantiates a new unknown lexeme exception with enough information to make a suitable error message.
    /// </summary>
    /// <param name="lexemePosition">The position of the unknown symbol within the input boolean expression.</param>
    /// <param name="booleanExpression">The input boolean expression.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemePosition"/> or <paramref name="booleanExpression"/> is null.</exception>
    public UnknownLexemeException(LexemePosition lexemePosition,  string booleanExpression) {
        if (lexemePosition is null) throw new ArgumentNullException(nameof(lexemePosition));
        if (booleanExpression is null) throw new ArgumentNullException(nameof(booleanExpression));

        //Get the unknown symbol by using the lexeme position and the boolean expression.
        string unknownSymbol = booleanExpression.Substring(lexemePosition.StartPosition, lexemePosition.Length);
        Message = $"The symbol '{unknownSymbol}' was not recognised by the lexer";   //Form a message from the unknown token.
    }
}