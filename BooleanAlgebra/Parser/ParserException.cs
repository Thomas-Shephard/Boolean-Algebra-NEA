namespace BooleanAlgebra.Parser;
/// <summary>
/// Holds the position of an exception, thrown by the parser when it experiences a syntax error
/// </summary>
public class ParserException : Exception {
    /// <summary>
    /// The position of the error within the input text.
    /// </summary>
    public LexemePosition LexemePosition { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserException"/> class with a <paramref name="lexemePosition"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="lexemePosition">The position of the error within the input text.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemePosition"/> or <paramref name="message"/> is null.</exception>
    public ParserException(LexemePosition lexemePosition, string message) : base(message) {
        if (message is null) throw new ArgumentNullException(nameof(message));                      //Ensure that message is not null
        LexemePosition = lexemePosition ?? throw new ArgumentNullException(nameof(lexemePosition)); //Ensure that lexemePosition is not null
    }
}