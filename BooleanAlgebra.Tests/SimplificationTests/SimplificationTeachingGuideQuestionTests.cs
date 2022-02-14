namespace BooleanAlgebra.Tests.SimplificationTests;
[TestClass]
public class SimplificationTeachingGuideQuestionTests {
    [TestMethod]
    public void Should_Simplify_Question_01_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("(!A + B) . (A + !B)", "(A . B) + !(A + B)"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_02_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("A . B + B", "B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_03_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("A . B + A . !B", "A"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_04_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("B . (A + !A)", "B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_05_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A + !(B . A))", "A . B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_06_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(1 . B)", "!B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_07_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(A . B) + !(A . !B)", "1"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_08_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(A + B + !A)", "0"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_09_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("A . (B + 1)", "A"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_10_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("(X + Y) . (X + !Y)", "X"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_11_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A . !B) + A + !B", "1"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_12_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A + !C) . !(A + B)", "0"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_13_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!(A + B) . !(!A + C))", "1"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_14_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(D . !E + !E . (D + !D))", "E"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_15_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(D . (E + !D))", "!(D.E)"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_16_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A . B + A . !(A + !B))", "A + !B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_17_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("B . (A + !B)", "A . B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_18_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!(D + !E) + !E)", "D . E"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_19_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("(!A + A . B) . !B", "!(A + B)"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_20_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A + !B) + B . !A", "B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_21_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!D + !E)", "D . E"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_22_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(X . !Y + !X . !Y + !X . Y)", "X . Y"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_23_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!B . !(!A + !B)", "0"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_24_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!D + !!E)", "D . !E"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_25_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("X . (!X + Y)", "X . Y"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_26_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A . !B)", "A + B"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_27_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!((D . !E) . D) + !E", "1"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_28_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("A . B . !C + A . !C", "A . !C"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_29_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!A + !B) + A . !B", "A"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_30_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!A + !(B + A)", "!A"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_31_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("X + !(!Y . !Z)", "X + Y + Z"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_32_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(!X . !Y) + !X", "1"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_33_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(X . !Y + !X . !Y)", "Y"));
    }
    
    [TestMethod]
    public void Should_Simplify_Question_34_From_Teaching_Guide() {
        Assert.IsTrue(SimplificationTestUtils.Compare("!(A . C + A . B + B . !C + !A . C)", "!(C + B)"));
    }
}