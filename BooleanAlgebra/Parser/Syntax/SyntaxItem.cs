using System;

namespace BooleanAlgebra.Parser.Syntax {
    public abstract class SyntaxItem : IEquatable<SyntaxItem> {
        public abstract string Value { get; }
        public abstract override string ToString();
        public abstract bool Equals(SyntaxItem? other);
        public abstract override bool Equals(object? obj);
        public abstract override int GetHashCode();
    }
}