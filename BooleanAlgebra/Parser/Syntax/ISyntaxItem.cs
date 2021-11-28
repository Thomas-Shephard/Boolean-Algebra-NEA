namespace BooleanAlgebra.Parser.Syntax; 
public interface ISyntaxItem : IEquatable<ISyntaxItem> {
    public Identifier Identifier { get; }
    public int GetCost();
    public string ToString(int higherLevelPrecedence);
}

public interface ISingleDaughterSyntaxItem : ISyntaxItem {
    public ISyntaxItem Daughter { get; }
}

public interface IMultipleDaughterSyntaxItem : ISyntaxItem {
    public ISyntaxItem[] Daughters { get; }
}