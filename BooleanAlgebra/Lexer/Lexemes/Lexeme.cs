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
    /// Initializes a new lexeme with an identifier and position.
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
        
        //The other lexeme is equal to this lexeme if all of the properties match.
        return Identifier.Equals(other.Identifier)
            && LexemePosition.Equals(other.LexemePosition);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;   //If the other object is null, it cannot be equal to this lexeme.
        if (ReferenceEquals(this, obj)) return true;    //If the other object is this lexeme, it is equal to this lexeme.
        
        //Compare the other object as a lexeme with a more specific comparison.
        //If the other object is not a lexeme, the comparison will fail.
        return Equals(obj as Lexeme);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Identifier, LexemePosition);
    }
}