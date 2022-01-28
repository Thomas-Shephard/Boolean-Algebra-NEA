using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BooleanAlgebra.Tests; 

[TestClass]
public class MiscQuestions {
    [TestMethod]
    public void Test_01() {
        Assert.IsTrue(SimplificationUtils.Compare("A + !D + ((!A + !C) . (!B + !C + !D) . (!A + !B + !D))", "A + !(B . C . D)"));
    }
    
    [TestMethod]
    public void Test_02() {
         Assert.IsTrue(SimplificationUtils.Compare("(!(B . D) + A . !D) . B + C . D", "B . !D + C . D"));
    }
    
    [TestMethod]
    public void Test_03() {
        Assert.IsTrue(SimplificationUtils.Compare("A + A . B + A . C + A", "A"));
    }
    
    [TestMethod]
    public void Test_04() {
        Assert.IsTrue(SimplificationUtils.Compare("!A + !B + C", "!(A . B) + C"));
    }
    
    [TestMethod]
    public void Test_05() {
        Assert.IsTrue(SimplificationUtils.Compare("!A + !B + !C", "!(A . B . C)"));
    }
    
    [TestMethod]
    public void Test_06() {
        Assert.IsTrue(SimplificationUtils.Compare("!A + !B + !C + !D", "!(A . B . C . D)"));
    }
    
    [TestMethod]
    public void Test_07() {
        Assert.IsTrue(SimplificationUtils.Compare("!A + !B + !C + !D + !E", "!(A . B . C . D . E)"));
    }
    
    [TestMethod]
    public void Test_08() {
        Assert.IsTrue(SimplificationUtils.Compare("!A + !B + !C + !D + !E + !F + !G", "!(A . B . C . D . E . F . G)"));
    }
    
    [TestMethod]
    public void Test_09() {
        Assert.IsTrue(SimplificationUtils.Compare("A + A + A", "A"));
    }
}