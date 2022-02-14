namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerMultipleLexemeTests {
    [TestMethod]
    public void Should_Create_OR_Lexeme_NOT_Lexeme_And_AND_Lexeme() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("+"), 0, 1),
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("!"), 1, 2),
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("."), 3, 4)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("+! .").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Multiple_VARIABLE_Lexemes() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("A"), 0, 1, "A"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("B"), 2, 3, "B"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("C"), 4, 5, "C"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("D"), 6, 7, "D"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("E"), 8, 9, "E")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("A B C D E").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Multiple_BOOLEAN_CONSTANT_Lexemes() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("0"), 0, 1, "0"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("0"), 2, 3, "0"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("1"), 4, 5, "1"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("1"), 6, 7, "1"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("0"), 8, 9, "0")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("0 0 1 1 0").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_OPERATOR_Lexemes_And_VARIABLE_Lexemes() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("."), 0, 1),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("TEST"), 2, 6, "TEST"),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("B"), 7, 8, "B"),
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("+"), 9, 10),
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("D"), 11, 12, "D")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer(". TEST B + D").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
}