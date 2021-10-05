﻿using System;

namespace BooleanAlgebra.Syntax {
    public abstract class SyntaxItem : IEquatable<SyntaxItem> {
        public abstract override string ToString();
        public abstract bool Equals(SyntaxItem? other);
        public abstract override bool Equals(object? obj);
        public abstract override int GetHashCode();
    }
}