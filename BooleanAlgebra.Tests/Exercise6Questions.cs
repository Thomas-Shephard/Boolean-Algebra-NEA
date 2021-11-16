using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BooleanAlgebra.Tests;

[TestClass]
public class Exercise6Questions {
    [TestMethod]
    public void Test_01() {
        Assert.IsTrue(SimplificationUtils.Compare("(!A + B) . (A + !B)", "(A . B) + !(A + B)"));
    }
    
    [TestMethod]
    public void Test_02() {
        Assert.IsTrue(SimplificationUtils.Compare("A . B + B", "B"));
    }
    
    [TestMethod]
    public void Test_03() {
        Assert.IsTrue(SimplificationUtils.Compare("A . B + A . !B", "A"));
    }
    
    [TestMethod]
    public void Test_04() {
        Assert.IsTrue(SimplificationUtils.Compare("B . (A + !A)", "B"));
    }
    
    [TestMethod]
    public void Test_05() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!A + !(B.A))", "A . B"));
    }
    
    [TestMethod]
    public void Test_06() {
        Assert.IsTrue(SimplificationUtils.Compare("!(1 . B)", "!B"));
    }
    
    [TestMethod]
    public void Test_07() {
        Assert.IsTrue(SimplificationUtils.Compare("!(A . B) + !(A . !B)", "1"));
    }
    
    [TestMethod]
    public void Test_08() {
        Assert.IsTrue(SimplificationUtils.Compare("!(A + B + !A)", "0"));
    }
    
    [TestMethod]
    public void Test_09() {
        Assert.IsTrue(SimplificationUtils.Compare("A . (B + 1)", "A"));
    }
    
    [TestMethod]
    public void Test_10() {
        Assert.IsTrue(SimplificationUtils.Compare("(X + Y) . (X + !Y)", "X"));
    }
    
    [TestMethod]
    public void Test_11() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!A . !B) + A + !B", "1"));
    }
    
    [TestMethod]
    public void Test_12() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!A + !C) . !(A + B)", "0"));
    }
    
    [TestMethod]
    public void Test_13() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!(A + B) . !(!A + C))", "1"));
    }
    
    [TestMethod]
    public void Test_14() {
        Assert.IsTrue(SimplificationUtils.Compare("!(D . !E + !E . (D + !D))", "E"));
    }
    
    [TestMethod]
    public void Test_15() {
        Assert.IsTrue(SimplificationUtils.Compare("!(D . (E + !D))", "!(D.E)"));
    }
    
    [TestMethod]
    public void Test_16() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!A . B + A . !(A + !B))", "A + !B"));
    }
    
    [TestMethod]
    public void Test_17() {
        Assert.IsTrue(SimplificationUtils.Compare("B . (A + !B)", "A . B"));
    }
    
    [TestMethod]
    public void Test_18() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!(D + !E) + !E)", "D . E"));
    }
    
    [TestMethod]
    public void Test_19() {
        Assert.IsTrue(SimplificationUtils.Compare("(!A + A . B) . !B", "!(A + B)"));
    }
    
    [TestMethod]
    public void Test_20() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!A + !B) + B . !A", "B"));
    }
    
    [TestMethod]
    public void Test_21() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!D + !E)", "D . E"));
    }
    
    [TestMethod]
    public void Test_22() {
        Assert.IsTrue(SimplificationUtils.Compare("!(X . !Y + !X . !Y + !X . Y)", "X . Y"));
    }
    
    [TestMethod]
    public void Test_23() {
        Assert.IsTrue(SimplificationUtils.Compare("!B . !(!A + !B)", "0"));
    }
    
    [TestMethod]
    public void Test_24() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!D + !!E)", "D . !E"));
    }
    
    [TestMethod]
    public void Test_25() {
        Assert.IsTrue(SimplificationUtils.Compare("X . (!X + Y)", "X . Y"));
    }
    
    [TestMethod]
    public void Test_26() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!A . !B)", "A + B"));
    }
    
    [TestMethod]
    public void Test_27() {
        Assert.IsTrue(SimplificationUtils.Compare("!((D . !E) . D) + !E", "1"));
    }
    
    [TestMethod]
    public void Test_28() {
        Assert.IsTrue(SimplificationUtils.Compare("A . B . !C + A . !C", "A . !C"));
    }
    
    [TestMethod]
    public void Test_29() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!A + !B) + A . !B", "A"));
    }
    
    [TestMethod]
    public void Test_30() {
        Assert.IsTrue(SimplificationUtils.Compare("!A + !(B + A)", "!A"));
    }
    
    [TestMethod]
    public void Test_31() {
        Assert.IsTrue(SimplificationUtils.Compare("X + !(!Y . !Z)", "X + Y + Z"));
    }
    
    [TestMethod]
    public void Test_32() {
        Assert.IsTrue(SimplificationUtils.Compare("!(!X . !Y) + !X", "1"));
    }
    
    [TestMethod]
    public void Test_33() {
        Assert.IsTrue(SimplificationUtils.Compare("!(X . !Y + !X . !Y)", "Y"));
    }
    
    [TestMethod]
    public void Test_34() {
        Assert.IsTrue(SimplificationUtils.Compare("!(A . C + A . B + B . !C + !A . C)", "!(C + B)"));
    }
}