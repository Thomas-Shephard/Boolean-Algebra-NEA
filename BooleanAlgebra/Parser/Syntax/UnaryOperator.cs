namespace BooleanAlgebra.Parser.Syntax; 
public class UnaryOperator : ISingleDaughterSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem Daughter { get; }

    public UnaryOperator(Identifier identifier, ISyntaxItem daughter) {
        Identifier = identifier;
        Daughter = daughter;
    }
    
    public int GetCost() {
        return 1 + Daughter.GetCost();
    }
    
    public string ToString(int higherLevelPrecedence) {
        return $"{Identifier.Name}{Daughter.ToString(Identifier.Precedence)}";
    }
    
    public override string ToString() {
        return ToString(0);
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return other is UnaryOperator otherUnaryOperator
            && Identifier.Equals(otherUnaryOperator.Identifier) 
            && Daughter.Equals(otherUnaryOperator.Daughter);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as UnaryOperator);
    }
    
    public override int GetHashCode() {
        return HashCode.Combine(Identifier, Daughter);
    }
}