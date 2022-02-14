namespace BooleanAlgebra.Tests.ParserTests; 
[TestClass]
public class ParserBinaryOperatorTests {
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Two_Child_Node_Binary_Operator() {
        const string inputBooleanExpression = "A + B";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B")
        };
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("+"), childNodes);

        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Three_Child_Node_Binary_Operator() {
        const string inputBooleanExpression = "A + B + C";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };
        
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("+"), childNodes);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Multiple_Binary_Operators() {
        const string inputBooleanExpression = "A + 1 . C";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("1"), "1"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };

        childNodes = new ISyntaxItem[] {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes)
        };
        
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("OR"), childNodes);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Unary_Operator_And_Nested_Binary_Operators() {
        const string inputBooleanExpression = "!(A + B . C)";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };

        childNodes = new ISyntaxItem[] {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes)
        };
        
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("OR"), childNodes);
        expectedSyntaxTree = new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), expectedSyntaxTree);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }

    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Binary_Operator_With_No_Child_Nodes() {
        const string inputBooleanExpression = ".";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Binary_Operator_With_Only_Prepended_Child_Node() {
        const string inputBooleanExpression = "A +";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Binary_Operator_With_Only_Appended_Child_Node() {
        const string inputBooleanExpression = ". B";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Binary_Operator_With_Missing_Child_Node() {
        const string inputBooleanExpression = "A . + B";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
}