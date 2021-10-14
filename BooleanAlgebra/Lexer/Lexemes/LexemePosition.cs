using System;

namespace BooleanAlgebra.Lexer.Lexemes {
    /// <summary>
    /// Holds the startPosition and endPosition of a lexeme.
    /// </summary>
    public class LexemePosition : IEquatable<LexemePosition> {
        /// <summary>
        /// The startPosition of the lexeme.
        /// </summary>
        public uint StartPosition { get; }
        /// <summary>
        /// The endPosition of the lexeme.
        /// </summary>
        public uint EndPosition { get; }
        /// <summary>
        /// The length of the lexeme.
        /// </summary>
        public uint Length => EndPosition - StartPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="LexemePosition"/> class with a <paramref name="startPosition"/> and <paramref name="endPosition"/>.
        /// </summary>
        /// <param name="startPosition">The startPosition of the lexeme.</param>
        /// <param name="endPosition">The endPosition of the lexeme.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="startPosition"/> is greater than <paramref name="endPosition"/>.</exception>
        public LexemePosition(uint startPosition, uint endPosition) {
            if (endPosition < startPosition)    //Ensure that the startPosition is not greater than the endPosition
                throw new ArgumentException($"The parameter {nameof(startPosition)} cannot be greater than the parameter {nameof(endPosition)}.");
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexemePosition"/> class with the same startPosition and endPosition.
        /// </summary>
        /// <param name="startAndEndPosition">The startPosition and endPosition of the lexeme.</param>
        public LexemePosition(uint startAndEndPosition) : this(startAndEndPosition, startAndEndPosition) { }
        
        public override string ToString() {
            return $"[{StartPosition}, {EndPosition}]";      //Outputs in the format [StartPosition, EndPosition]
        }
        
        public bool Equals(LexemePosition? other) {
            return other is not null                        //Check that the other lexeme is not null
                   && other.StartPosition == StartPosition  //Check that the startPositions are equal
                   && other.EndPosition == EndPosition;     //Check that the endPositions are equal
        }

        public override bool Equals(object? obj) {
            return Equals(obj as LexemePosition);
        }

        public override int GetHashCode() {
            return HashCode.Combine(StartPosition, EndPosition);
        }
    }
}