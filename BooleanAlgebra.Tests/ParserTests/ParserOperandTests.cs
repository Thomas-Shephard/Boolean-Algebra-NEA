namespace BooleanAlgebra.Tests.ParserTests; 
[TestClass]
public class ParserOperandTests {
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Single_Variable() {
        const string inputBooleanExpression = "A";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem expectedSyntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A");
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Single_Literal() {
        const string inputBooleanExpression = "0";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem expectedSyntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("0"), "0");
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Missing_Binary_Operator_1() {
        const string inputBooleanExpression = "0 0";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Missing_Binary_Operator_2() {
        const string inputBooleanExpression = "A 0";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
}