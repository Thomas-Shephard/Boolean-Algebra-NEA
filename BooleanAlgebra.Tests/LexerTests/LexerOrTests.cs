namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerOrTests {
    [TestMethod]
    public void Should_Create_OR_Lexeme_From_Plus() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("+"), 0, 1)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("+").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_OR_Lexeme_From_OR() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("OR"), 0, 2)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("OR").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_OR_Lexeme_From_or() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("or"), 0, 2)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("or").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_OR_Lexeme_From_or_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("or"), 2, 4)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("  or").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
}