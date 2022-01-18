namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds the information about a lexeme that has been created by the lexer.
/// A contextual lexeme is an extended form of the lexeme that also contains the context of the lexeme.
/// </summary>
public sealed class ContextualLexeme : Lexeme, IEquatable<ContextualLexeme> {
    /// <summary>
    /// The string representation of a lexeme.
    /// </summary>
    public string LexemeValue { get; }

    /// <summary>
    /// Instantiates a new contextual lexeme with the additional context parameter.
    /// </summary>
    /// <param name="identifier">The type that the lexeme is of.</param>
    /// <param name="lexemePosition">The position of the lexeme within the input string.</param>
    /// <param name="lexemeValue">The string representation of the lexeme.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="identifier"/> or <paramref name="lexemePosition"/> or <paramref name="lexemeValue"/> is null.</exception>
    public ContextualLexeme(Identifier identifier, LexemePosition lexemePosition, string lexemeValue) : base(identifier, lexemePosition) {
        LexemeValue = lexemeValue ?? throw new ArgumentNullException(nameof(lexemeValue));
    }

    public override string ToString() {
        return $"[{Identifier}, {LexemeValue}, {LexemePosition}]";  //Outputs in the format [Identifier, LexemeValue, LexemePosition].
    }

    public bool Equals(ContextualLexeme? other) {
        if (ReferenceEquals(null, other)) return false;     //If the other contextual lexeme is null, it cannot be equal to this contextual lexeme.
        if (ReferenceEquals(this, other)) return true;      //If the other contextual lexeme is this contextual lexeme, it is equal to this contextual lexeme.
        
        //The other contextual lexeme is equal to this contextual lexeme if all of the properties match.
        return Identifier.Equals(other.Identifier)
            && LexemePosition.Equals(other.LexemePosition)
            && LexemeValue == other.LexemeValue;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;   //If the other object is null, it cannot be equal to this contextual lexeme.
        if (ReferenceEquals(this, obj)) return true;    //If the other object is this contextual lexeme, it is equal to this contextual lexeme.
        
        //Compare the other object as a contextual lexeme with a more specific comparison.
        //If the other object is not a contextual lexeme, the comparison will fail.
        return Equals(obj as ContextualLexeme);
    }

    public override int GetHashCode() {
        //Provides a hash code for the contextual lexeme that takes the properties from the current object.
        return  HashCode.Combine(Identifier, LexemePosition, LexemeValue);
    }
}