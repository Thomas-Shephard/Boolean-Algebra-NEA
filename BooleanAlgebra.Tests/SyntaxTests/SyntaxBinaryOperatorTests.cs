namespace BooleanAlgebra.Tests.SyntaxTests; 
[TestClass]
public class SyntaxBinaryOperatorTests {
    [TestMethod]
    public void Should_Allow_Creation_Of_Binary_Operator_With_Two_Child_Nodes() {
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B")
        };
        
        _ = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes);
    }
    
    [TestMethod]
    public void Should_Allow_Creation_Of_Binary_Operator_With_Three_Child_Nodes() {
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("B"), "B"),
            new Operand(LexerTestUtils.GetIdentifierFromValue("C"), "C")
        };
        
        _ = new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes);
    }
    
    [TestMethod]
    public void Should_Disallow_Creation_Of_Binary_Operator_With_One_Child_Node() {
        ISyntaxItem[] childNodes = {
            new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A")
        };

        Assert.ThrowsException<ArgumentException>(() => new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), childNodes));
    }
    
    [TestMethod]
    public void Should_Disallow_Creation_Of_Binary_Operator_With_Null_Child_Nodes() {
        Assert.ThrowsException<ArgumentNullException>(() => new BinaryOperator(LexerTestUtils.GetIdentifierFromValue("AND"), null));
    }
}