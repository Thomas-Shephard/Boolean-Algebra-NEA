namespace BooleanAlgebra.Tests.ParserTests; 
[TestClass]
public class ParserUnaryOperatorTests {
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Unary_Operator() {
        const string inputBooleanExpression = "!A";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem expectedSyntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A");
        expectedSyntaxTree = new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), expectedSyntaxTree);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Multiple_Nested_Unary_Operators() {
        const string inputBooleanExpression = "!!0";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem expectedSyntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("0"), "0");
        expectedSyntaxTree = new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), expectedSyntaxTree);
        expectedSyntaxTree = new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), expectedSyntaxTree);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Nested_Unary_Operator() {
        const string inputBooleanExpression = "!A + B";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A")),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B")
        };
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("+"), childNodes);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }

    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Missing_Child_Node() {
        const string inputBooleanExpression = "!";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }

    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Missing_Binary_Operator() {
        const string inputBooleanExpression = "A !A";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Missing_Binary_Operator_And_Missing_Child_Node() {
        const string inputBooleanExpression = "!A !";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
}