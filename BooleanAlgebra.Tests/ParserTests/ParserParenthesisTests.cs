namespace BooleanAlgebra.Tests.ParserTests; 
[TestClass]
public class ParserParenthesisTests {
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Parenthesised_Variable() {
        const string inputBooleanExpression = "(A)";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem expectedSyntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A");
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Parenthesised_Literal() {
        const string inputBooleanExpression = "(1)";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem expectedSyntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("1"), "1");
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Unnecessary_Parentheses() {
        const string inputBooleanExpression = "A + (A . B)";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B")
        };

        childNodes = new ISyntaxItem[] {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes)
        };
        
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("OR"), childNodes);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Necessary_Parentheses_1() {
        const string inputBooleanExpression = "A . (A + B)";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B")
        };

        childNodes = new ISyntaxItem[] {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("OR"), childNodes)
        };
        
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }

    [TestMethod]
    public void Should_Allow_Creation_Of_Syntax_Tree_With_Necessary_Parentheses_2() {
        const string inputBooleanExpression = "A . (B + C) . D";
        ISyntaxItem parsedSyntaxTree = new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse();
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };

        childNodes = new ISyntaxItem[] {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("OR"), childNodes),
            new Operand(LexerTestUtils.GetIdentifierFromValue("D"), "D")
        };
        
        ISyntaxItem expectedSyntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes);
        
        Assert.AreEqual(expectedSyntaxTree, parsedSyntaxTree);
    }

    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Missing_Expression_In_Parentheses() {
        const string inputBooleanExpression = "()";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Unbalanced_Parentheses() {
        const string inputBooleanExpression = "(A))";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_With_Missing_Binary_Operator() {
        const string inputBooleanExpression = "A (A)";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_Without_Closing_Parenthesis() {
        const string inputBooleanExpression = "(";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
    
    [TestMethod]
    public void Should_Not_Allow_Creation_Of_Syntax_Tree_Without_Opening_Parenthesis() {
        const string inputBooleanExpression = ")";
        Assert.ThrowsException<ParserException>(() => new Parser.Parser(new Lexer.Lexer(inputBooleanExpression).Lex(), inputBooleanExpression).Parse());
    }
}