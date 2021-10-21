using System;
using System.Collections.Generic;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Simplification {
    public class SimplificationRule {
        public SyntaxItem LeftHandSide { get; }
        public SyntaxItem RightHandSide { get; }
        public string Message { get; }

        public SimplificationRule(string leftHandSide, string rightHandSide, string message) {
          /*  if (!Lexer.Lexer.Lex(leftHandSide, out List<Lexeme> leftHandSideLexemes))
                throw new ArgumentException(nameof(leftHandSide));
            if (!Lexer.Lexer.Lex(rightHandSide, out List<Lexeme> rightHandSideLexemes))
                throw new ArgumentException(nameof(rightHandSide));
            LeftHandSide = Parser.Parser.Parse(leftHandSideLexemes, true)
                           ?? throw new ArgumentException(nameof(leftHandSideLexemes));
            RightHandSide = Parser.Parser.Parse(rightHandSideLexemes, true)
                           ?? throw new ArgumentException(nameof(leftHandSideLexemes));
            Message = message ?? throw new ArgumentNullException(nameof(message));*/
        }

        private static IEnumerable<SimplificationRule>? _simplificationRules;

        public static IEnumerable<SimplificationRule> GetSimplificationRules() {
            //_simplificationRules ??= GenerateSimplificationRules();
            return GenerateSimplificationRules();
        }

        private static IEnumerable<SimplificationRule> GenerateSimplificationRules() {
            return new[] {
                //AND
                new SimplificationRule("A AND A", "A", "Idempotent Law"),
                new SimplificationRule("A AND NOT A", "0", "Complement Law"),
                new SimplificationRule("A AND 0", "0", "Annulment Law"),
                new SimplificationRule("A AND 1", "A", "Identity Law"),
                //OR
                new SimplificationRule("A OR A", "A", "Idempotent Law"),
                new SimplificationRule("A OR NOT A", "1", "Complement Law"),
                new SimplificationRule("A OR 0", "A", "Identity Law"),
                new SimplificationRule("A OR 1", "1", "Annulment Law"),
                //NOT
                new SimplificationRule("NOT 0", "1", "NOT 0 is 1"),
                new SimplificationRule("NOT 1", "0", "NOT 1 is 0"),
                new SimplificationRule("NOT NOT A", "A", "Involution Law"),
                //De Morgan's Laws
                new SimplificationRule("NOT (A OR B)", "(NOT A) AND (NOT B)", "De Morgan's Law"),
                new SimplificationRule("NOT (A AND B)", "(NOT A) OR (NOT B)", "De Morgan's Law"),
                //Distributive Laws
                //new SimplificationRule("A AND (B OR C)", "(A AND B) OR (A AND C)", "Distributive Law"),
                //new SimplificationRule("A OR (B AND C)", "(A OR B) AND (A OR C)", "Distributive Law"),
            };
        }
    }
}