namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds the information about a lexeme that has been created by the lexer.
/// A contextual lexeme is an extended form of the lexeme that also contains the context of the lexeme.
/// </summary>
public sealed class ContextualLexeme : Lexeme {
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
        //This method is not designed to be used in production code and is only for debugging purposes.
        if (Debugger.IsAttached) {
            return $"[{Identifier.Name}, {LexemeValue}, {LexemePosition}]"; //Outputs in the format [Identifier, LexemeValue, LexemePosition].
        }
        throw new InvalidOperationException("This method is only for debugging purposes.");
    }

    public override bool Equals(Lexeme? other) {
        if (ReferenceEquals(null, other)) return false;     //If the other lexeme is null, it cannot be equal to this contextual lexeme.
        if (ReferenceEquals(this, other)) return true;      //If the other lexeme is this contextual lexeme, it is equal to this contextual lexeme.
        
        //The lexemes are only equal if the other lexeme is a contextual lexeme and if their properties are equal to each other.
        return other is ContextualLexeme otherContextualLexeme 
            && Identifier.Equals(other.Identifier)
            && LexemePosition.Equals(other.LexemePosition)
            && LexemeValue == otherContextualLexeme.LexemeValue;
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this identifier:
        //  1. If the other object is a contextual lexeme.
        //  2. If the other contextual lexeme satisfies the equality comparison.
        return obj is ContextualLexeme otherContextualLexeme && Equals(otherContextualLexeme);
    }

    public override int GetHashCode() {
        //Provides a hash code for the contextual lexeme that takes the properties from the current object.
        return  HashCode.Combine(Identifier, LexemePosition, LexemeValue);
    }
}