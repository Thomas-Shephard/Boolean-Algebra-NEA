namespace BooleanAlgebra.Parser.Syntax; 
public class BinaryOperator : IMultipleDaughterSyntaxItem {
    public Identifier Identifier { get; }
    public ISyntaxItem[] Daughters { get; }

    public BinaryOperator(Identifier identifier, IEnumerable<ISyntaxItem> daughters) {
        Identifier = identifier;

        List<ISyntaxItem> modifiableSyntaxItems = daughters.ToList();
        for (int i = modifiableSyntaxItems.Count - 1; i >= 0; i--) {
            if (modifiableSyntaxItems[i] is not BinaryOperator binaryOperator ||
                !binaryOperator.Identifier.Equals(Identifier)) continue;
            modifiableSyntaxItems.RemoveAt(i);
            modifiableSyntaxItems.AddRange(binaryOperator.GetDaughterItems());
        }
        
        Daughters = modifiableSyntaxItems.ToArray();
    }
    
    public int GetCost() {
        return (Daughters.Length - 1) + Daughters.Sum(daughter => daughter.GetCost());
    }

    public string ToString(int higherLevelPrecedence) {
        bool parenthesesRequired = higherLevelPrecedence > Identifier.Precedence;
        
        StringBuilder stringBuilder = new();
        if (parenthesesRequired) {
            stringBuilder.Append('(');
        }

        for (int i = 0; i < Daughters.Length; i++) {
            if (i > 0) {
                stringBuilder.Append(' ').Append(Identifier.Name).Append(' ');
            }
            stringBuilder.Append(Daughters[i].ToString(Identifier.Precedence));
        }
        
        if (parenthesesRequired) {
            stringBuilder.Append(')');
        }
        
        return stringBuilder.ToString();
    }

    public override string ToString() {
        return ToString(0);
    }

    public bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return other is BinaryOperator otherBinaryOperator
            && Identifier.Equals(otherBinaryOperator.Identifier)
            && Daughters.SequenceEqualsIgnoreOrder(otherBinaryOperator.Daughters);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as BinaryOperator);
    }
    
    public override int GetHashCode() {
        return HashCode.Combine(Identifier, Daughters);
    }
}