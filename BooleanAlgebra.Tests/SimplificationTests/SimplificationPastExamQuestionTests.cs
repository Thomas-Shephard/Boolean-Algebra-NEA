namespace BooleanAlgebra.Tests.SimplificationTests; 
[TestClass]
public class SimplificationPastExamQuestionTests {
    [TestMethod]
    public void Should_Simplify_Question_6_3_From_AS_2019() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A . !B)", "A + B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_6_4_From_AS_2019() {
        Assert.IsTrue(SimplificationTestUtils.Compare("A . (A + C) . !A + !(!A . !(A . B))", "A"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_8_4_From_ALevel_2019() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!(!B . A) . !B) + A . B", "A + B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_5_3__From_AS_2020() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!(A + B . !B) + C . A)", "A . !C"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_6_4_From_ALevel_2020() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!(A . (A + 1)) . !B) . !(!A + !(B + 0))", "A . B"));
    }
}