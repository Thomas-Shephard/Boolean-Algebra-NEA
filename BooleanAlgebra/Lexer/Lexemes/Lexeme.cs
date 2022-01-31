namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds the information about a lexeme that has been created by the lexer.
/// </summary>
public class Lexeme : IEquatable<Lexeme> {
    /// <summary>
    /// The type of the lexeme.
    /// </summary>
    public Identifier Identifier { get; }
    /// <summary>
    /// The position of a lexeme within the input string.
    /// </summary>
    public LexemePosition LexemePosition { get; }

    /// <summary>
    /// Initialises a new lexeme with an identifier and position.
    /// </summary>
    /// <param name="identifier">The type that the lexeme is of.</param>
    /// <param name="lexemePosition">The position of the lexeme within the input string.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="identifier"/> or <paramref name="lexemePosition"/> is null.</exception>
    public Lexeme(Identifier identifier, LexemePosition lexemePosition) {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        LexemePosition = lexemePosition ?? throw new ArgumentNullException(nameof(lexemePosition));
    }

    public override string ToString() {
        //This method is not designed to be used in production code and is only for debugging purposes.
        if (Debugger.IsAttached) {
            return$"[{Identifier}, {LexemePosition}]"; //Outputs in the format [Identifier, LexemePosition].
        }
        throw new InvalidOperationException("This method is only for debugging purposes.");
    }

    public bool Equals(Lexeme? other) {
        if (ReferenceEquals(null, other)) return false; //If the other lexeme is null, it cannot be equal to this lexeme
        if (ReferenceEquals(this, other)) return true;  //If the other lexeme is this lexeme, it is equal to this lexeme
        
        //The lexemes are only equal if their properties are equal to each other.
        return Identifier.Equals(other.Identifier)
            && LexemePosition.Equals(other.LexemePosition);
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this lexeme:
        //  1. If the other object is a lexeme.
        //  2. If the other lexeme satisfies the equality comparison.
        return obj is Lexeme otherLexeme && Equals(otherLexeme);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Identifier, LexemePosition);
    }
}