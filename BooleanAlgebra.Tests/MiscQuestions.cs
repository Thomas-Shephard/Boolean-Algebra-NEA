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
}