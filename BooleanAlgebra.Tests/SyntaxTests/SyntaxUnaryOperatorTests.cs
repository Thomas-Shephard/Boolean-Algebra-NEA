namespace BooleanAlgebra.Tests.SyntaxTests; 
[TestClass]
public class SyntaxUnaryOperatorTests {
    [TestMethod]
    public void Should_Allow_Creation_Of_Unary_Operator_With_One_Child_Node() {
        _ = new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A"));
    }
    
    [TestMethod]
    public void Should_Disallow_Creation_Of_Unary_Operator_With_Null_Child_Node() {
        Assert.ThrowsException<ArgumentNullException>(() => new UnaryOperator(LexerTestUtils.GetIdentifierFromValue("NOT"), null));
    }
}