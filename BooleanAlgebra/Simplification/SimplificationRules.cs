using System.Collections.Generic;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Simplification {
    public class SimplificationRules {
        public SyntaxItem SyntaxItem { get; }
        public SyntaxItem Equivalence { get; }
        public string Message { get; }

        public SimplificationRules(SyntaxItem syntaxItem, SyntaxItem equivalence, string message) {
            SyntaxItem = syntaxItem;
            Equivalence = equivalence;
            Message = message;
        }

        
        public static SimplificationRules[] GetSimplificationRules() {
            return new[] {
                new SimplificationRules(new UnaryOperator("NOT", new UnaryOperator("NOT", new ArbitrarySyntaxItem("A"))), new ArbitrarySyntaxItem("A"), "Involution Law - NOT NOT A is A"),
                new SimplificationRules(new UnaryOperator("NOT", new Operand("0")), new Operand("1"), "NOT 0 is 1"),
                new SimplificationRules(new UnaryOperator("NOT", new Operand("1")), new Operand("0"), "NOT 1 is 0"),
                new SimplificationRules(new BinaryOperator("AND", new ArbitrarySyntaxItem("A"),  new ArbitrarySyntaxItem("A")), new ArbitrarySyntaxItem("A"), "A AND A = A"),
                new SimplificationRules(new BinaryOperator("AND", new ArbitrarySyntaxItem("A"), new UnaryOperator("NOT", new ArbitrarySyntaxItem("A"))), new Operand("0"), "A AND NOT A = 0"),
                new SimplificationRules(new BinaryOperator("AND", new ArbitrarySyntaxItem("A"),  new Operand("0")), new Operand("0"), "A AND 0 = 0"),
                new SimplificationRules(new BinaryOperator("AND", new ArbitrarySyntaxItem("A"),  new Operand("1")), new ArbitrarySyntaxItem("A"), "A AND 1 = A"),
                new SimplificationRules(new BinaryOperator("OR", new ArbitrarySyntaxItem("A"),  new ArbitrarySyntaxItem("A")), new ArbitrarySyntaxItem("A"), "A OR A = A"),
                new SimplificationRules(new BinaryOperator("OR", new ArbitrarySyntaxItem("A"), new UnaryOperator("NOT", new ArbitrarySyntaxItem("A"))), new Operand("1"), "A OR NOT A = 1"),
                new SimplificationRules(new BinaryOperator("OR", new ArbitrarySyntaxItem("A"),  new Operand("0")), new ArbitrarySyntaxItem("A"), "A OR 0 = A"),
                new SimplificationRules(new BinaryOperator("OR", new ArbitrarySyntaxItem("A"),  new Operand("1")), new Operand("1"), "A OR 1 = 1"),
            };
        }
    }
}