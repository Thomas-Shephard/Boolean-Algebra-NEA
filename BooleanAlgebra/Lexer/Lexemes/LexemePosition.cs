namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds the information about a lexeme's location within the input string.
/// </summary>
public sealed class LexemePosition : IEquatable<LexemePosition> {
    /// <summary>
    /// The position of the first character of the lexeme within the input string.
    /// </summary>
    public int StartPosition { get; }
    /// <summary>
    /// The position of the last character of the lexeme within the input string.
    /// The end position can be the same as the start position if the lexeme has zero length.
    /// This is designed so an end of line / end of statement lexeme could be added in the future.
    /// </summary>
    public int EndPosition { get; }
    /// <summary>
    /// The length of the lexeme calculated as the difference between the start and end positions.
    /// The length can be zero if the lexeme has zero length.
    /// The length cannot be negative because of input validation.
    /// </summary>
    public int Length => EndPosition - StartPosition;

    /// <summary>
    /// Initialises a new instance of the <see cref="LexemePosition"/> class with a <paramref name="startPosition"/> and <paramref name="endPosition"/>.
    /// </summary>
    /// <param name="startPosition">The startPosition of the lexeme.</param>
    /// <param name="endPosition">The endPosition of the lexeme.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="startPosition"/> is greater than <paramref name="endPosition"/>.</exception>
    public LexemePosition(int startPosition, int endPosition) {
        // Ensure that the start position is not before the zeroth character of the input string.
        if(startPosition < 0)
            throw new ArgumentException($"The parameter {nameof(startPosition)} must be greater than or equal to 0.", nameof(startPosition));
        //Ensure that the end position is not before the start position of the lexeme.
        if (endPosition < startPosition)
            throw new ArgumentException($"The parameter {nameof(endPosition)} cannot be less than the parameter {nameof(startPosition)}.", nameof(endPosition));
        StartPosition = startPosition;
        EndPosition = endPosition;
    }
    
    public override string ToString() {
        //This method is not designed to be used in production code and is only for debugging purposes.
        if (Debugger.IsAttached) {
            return $"[{StartPosition}, {EndPosition}]"; //Outputs in the format [StartPosition, EndPosition].
        }
        throw new InvalidOperationException("This method is only for debugging purposes.");
    }

    public bool Equals(LexemePosition? other) {
        if (ReferenceEquals(null, other)) return false; //If the other lexeme position is null, it cannot be equal to this lexeme position.
        if (ReferenceEquals(this, other)) return true;  //If the other lexeme position is this lexeme position, it is equal to this lexeme position.
        
        //The lexeme positions are only equal if their properties are equal to each other.
        //The Length property is not checked because it is a calculated property.
        return other.StartPosition == StartPosition
            && other.EndPosition == EndPosition;
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this lexeme position:
        //  1. If the other object is a lexeme position.
        //  2. If the other lexeme position satisfies the equality comparison.
        return obj is LexemePosition otherLexemePosition && Equals(otherLexemePosition);
    }

    public override int GetHashCode() {
        //Provides a hash code for the lexeme position that takes the properties from the current object.
        return HashCode.Combine(StartPosition, EndPosition);
    }
}