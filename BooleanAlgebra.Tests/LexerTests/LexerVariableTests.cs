namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerVariableTests {
    [TestMethod]
    public void Should_Create_Single_Character_VARIABLE_From_A() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("A"), 0, 1, "A")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("A").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Multi_Character_VARIABLE_From_TEST() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("TEST"), 0, 4, "TEST")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("TEST").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    [TestMethod]
    public void Should_Create_Multi_Character_VARIABLE_From_TEST_And_Ignore_Prepended_Whitespace() {
        Lexeme[] expectedLexemes = {
            LexerTestUtils.GenerateContextualLexeme(LexerTestUtils.GetOperandIdentifierFromValue("TEST"), 2, 6, "TEST")
        };
        List<Lexeme> actualLexemes = new Lexer.Lexer("  TEST").Lex();
        
        Assert.IsTrue(expectedLexemes.SequenceEqual(actualLexemes));
    }
    
    
    [TestMethod]
    public void Should_Create_Different_Variables_From_A_And_B() {
        List<Lexeme> lexemes1 = new Lexer.Lexer("A").Lex();
        List<Lexeme> lexemes2 = new Lexer.Lexer("B").Lex();
        
        Assert.IsFalse(lexemes1.SequenceEqual(lexemes2));
    }
    
    [TestMethod]
    public void Should_Create_Different_Variables_From_Test_And_TEST() {
        List<Lexeme> lexemes1 = new Lexer.Lexer("Test").Lex();
        List<Lexeme> lexemes2 = new Lexer.Lexer("TEST").Lex();
        
        Assert.IsFalse(lexemes1.SequenceEqual(lexemes2));
    }
}