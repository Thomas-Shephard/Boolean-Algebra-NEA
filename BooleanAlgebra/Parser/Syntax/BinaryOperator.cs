namespace BooleanAlgebra.Parser.Syntax;
public class BinaryOperator : SyntaxItem {
    public override uint GetCost() {
        return (uint) (DaughterItems.Count - 1 + DaughterItems.Aggregate<SyntaxItem, uint>(0, (current, daughterItem) => current + daughterItem.GetCost()));
    }

    public override string Value { get; }
    public sealed override List<SyntaxItem> DaughterItems { get; set; }

    public BinaryOperator(string lexemeType, IEnumerable<SyntaxItem> syntaxItems) {
        if (syntaxItems is null)
            throw new ArgumentNullException(nameof(syntaxItems));
        List<SyntaxItem> enumerable = syntaxItems.ToList();
        if (enumerable.Count < 2)
            throw new ArgumentException("There must be at least two syntax items");
        Value = lexemeType ?? throw new ArgumentNullException(nameof(lexemeType));
        DaughterItems = enumerable;
        
        for (int i = DaughterItems.Count - 1; i >= 0; i--) {
            if (DaughterItems[i] is not BinaryOperator binaryOperator ||
                binaryOperator.Value != lexemeType) continue;
            DaughterItems.RemoveAt(i);
            binaryOperator.DaughterItems.ForEach(syntaxItem => DaughterItems.Add(syntaxItem));
        }
    }
 
    public override SyntaxItem Clone() {
        return new BinaryOperator(Value, DaughterItems);
    }

    public override string ToString() {
        StringBuilder stringBuilder = new("(");
        for (int i = 0; i < DaughterItems.Count; i++) {
            if (i != 0) 
                stringBuilder.Append(' ');
            stringBuilder.Append($"{DaughterItems[i]}");
            if (i < DaughterItems.Count - 1)
                stringBuilder.Append(' ').Append($"{Value}");
        }
        return stringBuilder.Append(')').ToString();
    }

    public override bool Equals(SyntaxItem? other) {
        return other is BinaryOperator otherBinaryOperator
            && Value == otherBinaryOperator.Value
            && DaughterItems.SequenceEqualsIgnoreOrder(otherBinaryOperator.DaughterItems);
    }

    public override bool Equals(object? other) {
        return Equals(other as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value, DaughterItems);
    }
}