namespace BooleanAlgebra.Parser.Syntax; 
public class RepeatingOperator : ISingleDaughterSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem Daughter { get; }

    public RepeatingOperator(Identifier identifier, ISyntaxItem daughter) {
        Identifier = identifier;
        Daughter = daughter;
    }
    
    public int GetCost() {
        return 1 + Daughter.GetCost();
    }
    
    public string ToString(int higherLevelPrecedence) {
        return string.Empty;
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return other is RepeatingOperator otherRepeatingOperator
            && Identifier.Equals(otherRepeatingOperator.Identifier) 
            && Daughter.Equals(otherRepeatingOperator.Daughter);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as RepeatingOperator);
    }
    
    public override int GetHashCode() {
        return HashCode.Combine(Identifier, Daughter);
    }
}