﻿using System;

namespace BooleanAlgebra.Lexer.Lexemes {
    public class LexemePosition : IEquatable<LexemePosition> {
        public uint StartPosition { get; }
        public uint EndPosition { get; }
        public uint Length => EndPosition - StartPosition;

        public LexemePosition(uint startPosition, uint endPosition) {
            if (endPosition < startPosition)
                throw new ArgumentException($"The parameter {nameof(startPosition)} cannot be greater than the parameter {nameof(endPosition)}.");
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public LexemePosition(uint startPosition) : this(startPosition, startPosition) { }
        
        public override string ToString() {
            return $"[{StartPosition}, {EndPosition}]";
        }
        
        public bool Equals(LexemePosition? other) {
            return other is not null
                   && other.StartPosition == StartPosition
                   && other.EndPosition == EndPosition;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as LexemePosition);
        }

        public override int GetHashCode() {
            return (StartPosition, EndPosition).GetHashCode();
        }
    }
}