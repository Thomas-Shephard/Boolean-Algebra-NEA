namespace BooleanAlgebra.Parser.Syntax; 
public class UnaryOperator : ISingleChildSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem Child { get; }

    public UnaryOperator(Identifier identifier, ISyntaxItem child) {
        Identifier = identifier;
        Child = child;
    }
    
    public int GetCost() {
        return 1 + Child.GetCost();
    }
    public string GetStringRepresentation(int higherLevelPrecedence = 0) {
        return $"{Identifier.Name}{Child.GetStringRepresentation(Identifier.Precedence)}";
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return other is UnaryOperator otherUnaryOperator
            && Identifier.Equals(otherUnaryOperator.Identifier) 
            && Child.Equals(otherUnaryOperator.Child);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as UnaryOperator);
    }
    
    public override int GetHashCode() {
        return HashCode.Combine(Identifier, Child);
    }
}