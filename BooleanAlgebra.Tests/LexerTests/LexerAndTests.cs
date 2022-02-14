namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerAndTests {
    [TestMethod]
    public void Should_Create_AND_Lexeme_From_Dot() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("."), 0, 1)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer(".").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_AND_Lexeme_From_AND() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("AND"), 0, 3)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("AND").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_AND_Lexeme_From_and() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("and"), 0, 3)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("and").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_AND_Lexeme_From_and_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("and"), 1, 4)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer(" and").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
}