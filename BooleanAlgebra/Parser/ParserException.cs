namespace BooleanAlgebra.Parser;
/// <summary>
/// Holds the position of an exception, thrown by the parser when it experiences a syntax error
/// </summary>
public class ParserException : Exception {
    /// <summary>
    /// The message that describes the exception.
    /// The message contains the lexeme that caused the exception as well as the start and end position of the lexeme.
    /// </summary>
    public override string Message { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="ParserException"/> class with a <paramref name="lexemePosition"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="lexemePosition">The position of the exception within the input text.</param>
    /// <param name="rawText">The input boolean expression as a string.</param>
    /// <param name="message">The message that describes the exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemePosition"/> or <paramref name="rawText"/> or <paramref name="message"/> is null.</exception>
    public ParserException(string message, LexemePosition lexemePosition, string rawText) {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (lexemePosition is null) throw new ArgumentNullException(nameof(lexemePosition));
        if (rawText is null) throw new ArgumentNullException(nameof(rawText));
        Message = $"{message} '{rawText.Substring(lexemePosition.StartPosition, lexemePosition.Length)}' (at position {lexemePosition.StartPosition} to {lexemePosition.EndPosition})";
    }
}