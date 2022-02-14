namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerParenthesisTests {
    [TestMethod]
    public void Should_Create_Opening_PARENTHESIS_Lexeme_From_Opening_Parenthesis() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("("), 0, 1)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("(").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Closing_PARENTHESIS_Lexeme_From_Closing_Parenthesis() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue(")"), 0, 1)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer(")").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Opening_PARENTHESIS_Lexeme_From_Opening_Parenthesis_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("("), 1, 2)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer(" (").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Closing_PARENTHESIS_Lexeme_From_Closing_Parenthesis_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue(")"), 2, 3)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("  )").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Both_Opening_And_Closing_PARENTHESIS_Lexemes_From_Opening_And_Closing_Parenthesis_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue("("), 0, 1),
            LexerTestUtils.GenerateContextFreeLexeme(LexerTestUtils.GetIdentifierFromValue(")"), 2, 3)
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("( )").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
}