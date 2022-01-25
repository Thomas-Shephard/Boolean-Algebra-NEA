namespace BooleanAlgebra.Parser.Syntax; 
public class RepeatingOperator : ISingleChildSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem Child { get; }

    public RepeatingOperator(Identifier identifier, ISyntaxItem child) {
        Identifier = identifier;
        Child = child;
    }
    
    public int GetCost() {
        return 1 + Child.GetCost();
    }
    public string GetStringRepresentation(int higherLevelPrecedence = 0) {
        return string.Empty;
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return other is RepeatingOperator otherRepeatingOperator
            && Identifier.Equals(otherRepeatingOperator.Identifier) 
            && Child.Equals(otherRepeatingOperator.Child);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as RepeatingOperator);
    }
    
    public override int GetHashCode() {
        return HashCode.Combine(Identifier, Child);
    }
}