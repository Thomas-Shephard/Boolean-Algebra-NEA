using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BooleanAlgebra.Lexer.Lexemes {
    public class LexemeIdentifier : IEquatable<LexemeIdentifier> {
        public string Name { get; }
        public bool IsContextRequired { get; }
        public Regex RegexPattern { get; }

        private LexemeIdentifier(string name, bool isContextRequired, Regex regexPattern) {
            Name = name;
            IsContextRequired = isContextRequired;
            RegexPattern = regexPattern;
        }

        public override string ToString() {
            return $"{Name}";
        }

        public bool Equals(LexemeIdentifier? other) {
            return other is not null
                   && Name == other.Name
                   && IsContextRequired == other.IsContextRequired
                   && RegexPattern.Equals(other.RegexPattern);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as LexemeIdentifier);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, IsContextRequired, RegexPattern);
        }

        public static IEnumerable<LexemeIdentifier> GetLexemeIdentifiers() {
            return new[] {
                OR,
                AND,
                NOT,
                LEFT_PARENTHESIS,
                RIGHT_PARENTHESIS,
                LITERAL,
                VARIABLE
            };
        }

        public static readonly LexemeIdentifier OR = new("OR", false, new Regex("^(?i)OR$"));
        public static readonly LexemeIdentifier AND = new("AND", false, new Regex("^(?i)AND$"));
        public static readonly LexemeIdentifier NOT = new("NOT", false, new Regex("^(?i)NOT$"));
        public static readonly LexemeIdentifier LEFT_PARENTHESIS = new("LEFT_PARENTHESIS", false, new Regex("^\\($"));
        public static readonly LexemeIdentifier RIGHT_PARENTHESIS = new("RIGHT_PARENTHESIS", false, new Regex("^\\)$"));
        public static readonly LexemeIdentifier LITERAL = new("LITERAL", true, new Regex("^(?i)(TRUE|FALSE)$"));
        public static readonly LexemeIdentifier VARIABLE = new("VARIABLE", true, new Regex("^[A-Za-z]+$"));
        public static readonly LexemeIdentifier UNKNOWN = new("UNKNOWN", true, new Regex(""));
    }
}