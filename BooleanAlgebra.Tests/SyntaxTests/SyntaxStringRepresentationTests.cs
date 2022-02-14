namespace BooleanAlgebra.Tests.SyntaxTests; 
[TestClass]
public class SyntaxStringRepresentationTests {
    [TestMethod]
    public void Should_Correctly_Display_Representation_Of_Literal() {
        ISyntaxItem syntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("0"), "0");
        const string expectedStringRepresentation = "0";
        Assert.AreEqual(expectedStringRepresentation, syntaxTree.GetStringRepresentation());
    }
    
    [TestMethod]
    public void Should_Correctly_Display_Representation_Of_Variable() {
        ISyntaxItem syntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A");
        const string expectedStringRepresentation = "A";
        Assert.AreEqual(expectedStringRepresentation, syntaxTree.GetStringRepresentation());
    }
    
    [TestMethod]
    public void Should_Not_Display_Parentheses_When_Not_Required_1() {
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };
        
        ISyntaxItem syntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes);
        const string expectedStringRepresentation = "A . B . C";
        Assert.AreEqual(expectedStringRepresentation, syntaxTree.GetStringRepresentation());
    }
    
    [TestMethod]
    public void Should_Not_Display_Parentheses_When_Not_Required_2() {
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("1"), "1"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };

        childNodes = new ISyntaxItem[] {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B"),
            new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes)
        };
        
        ISyntaxItem syntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("OR"), childNodes);
        const string expectedStringRepresentation = "A + B + A . 1 . C";
        Assert.AreEqual(expectedStringRepresentation, syntaxTree.GetStringRepresentation());
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Binary_Operator_With_Three_Child_Nodes() {
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("1"), "1"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };

        childNodes = new ISyntaxItem[] {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B"),
            new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("OR"), childNodes)
        };
        
        ISyntaxItem syntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes);
        const string expectedStringRepresentation = "A . B . (A + 1 + C)";
        Assert.AreEqual(expectedStringRepresentation, syntaxTree.GetStringRepresentation());
    }
}