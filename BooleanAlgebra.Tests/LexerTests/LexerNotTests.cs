namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerNotTests {
    [TestMethod]
    public void Should_Create_NOT_Lexeme_From_Exclamation_Mark() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("!"), 0, 1)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("!").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_NOT_Lexeme_From_NOT() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("NOT"), 0, 3)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("NOT").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_NOT_Lexeme_From_not() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("not"), 0, 3)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("not").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_NOT_Lexeme_From_not_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("not"), 4, 7)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("    not").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
}