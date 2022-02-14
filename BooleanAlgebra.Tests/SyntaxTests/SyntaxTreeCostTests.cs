namespace BooleanAlgebra.Tests.SyntaxTests; 
[TestClass]
public class SyntaxTreeCostTests {
    [TestMethod]
    public void Should_Correctly_Calculate_Cost_Of_Literal() {
        ISyntaxItem syntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("0"), "0");
        const int expectedCost = 1;
        Assert.AreEqual(expectedCost, syntaxTree.GetCost());
    }
    
    [TestMethod]
    public void Should_Correctly_Calculate_Cost_Of_Variable() {
        ISyntaxItem syntaxTree = new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A");
        const int expectedCost = 1;
        Assert.AreEqual(expectedCost, syntaxTree.GetCost());
    }
    
    [TestMethod]
    public void Should_Correctly_Calculate_Cost_From_Depth_1() {
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("0"), "0"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };
        
        ISyntaxItem syntaxTree = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes);
        const int expectedCost = 5;
        Assert.AreEqual(expectedCost, syntaxTree.GetCost());
    }
    
    [TestMethod]
    public void Should_Correctly_Calculate_Cost_From_Depth_2() {
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
        const int expectedCost = 9;
        Assert.AreEqual(expectedCost, syntaxTree.GetCost());
    }
    
    [TestMethod]
    public void Should_Correctly_Calculate_Cost_From_Depth_3() {
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
        syntaxTree = new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), syntaxTree);
        const int expectedCost = 10;
        Assert.AreEqual(expectedCost, syntaxTree.GetCost());
    }
}