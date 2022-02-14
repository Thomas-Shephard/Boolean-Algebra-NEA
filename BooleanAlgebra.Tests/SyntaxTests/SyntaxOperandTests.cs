namespace BooleanAlgebra.Tests.SyntaxTests; 
[TestClass]
public class SyntaxOperandTests {
    [TestMethod]
    public void Should_Allow_Creation_Of_Operand_With_Value() {
        _ = new Operand(LexerTestUtils.GetIdentifierFromValue("A"), "A");
    }
    
    [TestMethod]
    public void Should_Disallow_Creation_Of_Operand_With_Null_Value() {
        Assert.ThrowsException<ArgumentNullException>(() => new Operand(LexerTestUtils.GetIdentifierFromValue("A"), null));
    }
}