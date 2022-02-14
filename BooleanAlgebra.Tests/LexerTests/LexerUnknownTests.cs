namespace BooleanAlgebra.Tests.LexerTests; 
[TestClass]
public class LexerUnknownTests {
    [TestMethod]
    public void Should_Throw_Exception_For_Unknown_Variable() {
        Assert.ThrowsException<UnknownLexemeException>(() => new Lexer.Lexer("T_EST").Lex());
    }
    
    [TestMethod]
    public void Should_Throw_Exception_For_Unknown_Literal() {
        Assert.ThrowsException<UnknownLexemeException>(() => new Lexer.Lexer("2").Lex());
    }
    
    [TestMethod]
    public void Should_Throw_Exception_For_Unknown_Symbol() {
        Assert.ThrowsException<UnknownLexemeException>(() => new Lexer.Lexer("?").Lex());
    }
    
    [TestMethod]
    public void Should_Throw_Exception_For_Unknown_Symbol_At_Any_Location() {
        Assert.ThrowsException<UnknownLexemeException>(() => new Lexer.Lexer(". A B + D_").Lex());
    }
}