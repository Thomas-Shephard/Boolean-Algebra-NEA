namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds the identifier and position of a lexeme.
/// </summary>
public class Lexeme : IEquatable<Lexeme> {
    /// <summary>
    /// The identifier of a lexeme.
    /// </summary>
    public Identifier Identifier { get; }
    /// <summary>
    /// The position of a lexeme.
    /// </summary>
    public LexemePosition LexemePosition { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextualLexeme"/> class with an <paramref name="identifier"/> and <paramref name="lexemePosition"/>.
    /// </summary>
    /// <param name="identifier">The <see cref="Identifier"/> of the new <see cref="Lexeme"/>.</param>
    /// <param name="lexemePosition">The <see cref="LexemePosition"/> of the new <see cref="Lexeme"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="identifier"/> or <paramref name="lexemePosition"/> is null.</exception>
    public Lexeme(Identifier identifier, LexemePosition lexemePosition) {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));             //Ensure that the identifier is not null
        LexemePosition = lexemePosition ?? throw new ArgumentNullException(nameof(lexemePosition)); //Ensure that the lexemePosition is not null
    }

    public override string ToString() {
        return $"[{Identifier}, {LexemePosition}]"; //Outputs in the format [Identifier, LexemePosition]
    }

    public bool Equals(Lexeme? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Identifier.Equals(other.Identifier)          //Check that the identifiers are equal
            && LexemePosition.Equals(other.LexemePosition); //Check that the lexemePositions are equal
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return obj.GetType() == GetType() 
            && Equals(obj as Lexeme);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Identifier, LexemePosition);
    }
}