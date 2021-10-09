using System;
using System.Text.RegularExpressions;

namespace BooleanAlgebra.Identifiers {
    public class LexemeIdentifier : IEquatable<LexemeIdentifier> {
        public string Name { get; }
        public Regex Regex { get; }
        private string RegexPattern { get; }
        public bool IsContextRequired { get; }

        public LexemeIdentifier(string name, string regexPattern, bool isContextRequired) {
            Name = name;
            Regex = new Regex(regexPattern);
            RegexPattern = regexPattern;
            IsContextRequired = isContextRequired;
        }

        public override string ToString() {
            return $"{Name}";
        }

        public bool Equals(LexemeIdentifier? other) {
            return other is not null
                   && Name == other.Name
                   && RegexPattern == other.RegexPattern
                   && IsContextRequired == other.IsContextRequired;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as LexemeIdentifier);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, IsContextRequired, RegexPattern);
        }
    }
}