namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds the startPosition and endPosition of a lexeme.
/// </summary>
public sealed class LexemePosition : IEquatable<LexemePosition> {
    /// <summary>
    /// The startPosition of the lexeme.
    /// </summary>
    public int StartPosition { get; }
    /// <summary>
    /// The endPosition of the lexeme.
    /// </summary>
    public int EndPosition { get; }
    /// <summary>
    /// The length of the lexeme.
    /// </summary>
    public int Length => EndPosition - StartPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="LexemePosition"/> class with a <paramref name="startPosition"/> and <paramref name="endPosition"/>.
    /// </summary>
    /// <param name="startPosition">The startPosition of the lexeme.</param>
    /// <param name="endPosition">The endPosition of the lexeme.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="startPosition"/> is greater than <paramref name="endPosition"/>.</exception>
    public LexemePosition(int startPosition, int endPosition) {
        if(startPosition < 0)
            throw new ArgumentException($"The parameter {nameof(startPosition)} must be greater than or equal to 0.", nameof(startPosition));
        if (endPosition < startPosition)    //Ensure that the startPosition is not greater than the endPosition
            throw new ArgumentException($"The parameter {nameof(endPosition)} cannot be less than the parameter {nameof(startPosition)}.", nameof(endPosition));
        StartPosition = startPosition;
        EndPosition = endPosition;
    }
    
    public override string ToString() {
        return $"[{StartPosition}, {EndPosition}]"; //Outputs in the format [StartPosition, EndPosition]
    }

    public bool Equals(LexemePosition? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return other.StartPosition == StartPosition //Check that the startPositions are equal
            && other.EndPosition == EndPosition;    //Check that the endPositions are equal
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as LexemePosition);
    }

    public override int GetHashCode() {
        return HashCode.Combine(StartPosition, EndPosition);
    }
}