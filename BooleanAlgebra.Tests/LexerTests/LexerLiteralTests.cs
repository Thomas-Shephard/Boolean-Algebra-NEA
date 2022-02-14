namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerLiteralTests {
    [TestMethod]
    public void Should_Create_True_LITERAL_Lexeme_From_1() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("1"), 0, 1, "1")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("1").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_False_LITERAL_Lexeme_From_0() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("0"), 0, 1, "0")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("0").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_True_LITERAL_Lexeme_From_1_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("1"), 1, 2, "1")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer(" 1").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_False_LITERAL_Lexeme_From_0_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("0"), 2, 3, "0")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("  0").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Different_Lexemes_From_0_And_1() {
        List<Lexeme> lexemes1 = new Lexer.Lexer("0").Lex();
        List<Lexeme> lexemes2 = new Lexer.Lexer("1").Lex();
        
        Assert.IsFalse(lexemes1.SequenceEqual(lexemes2));
    }
}