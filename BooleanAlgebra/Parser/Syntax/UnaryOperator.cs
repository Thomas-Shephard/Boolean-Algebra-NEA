namespace BooleanAlgebra.Parser.Syntax;
/// <summary>
/// 
/// </summary>
public class UnaryOperator : SyntaxItem {
    /// <summary>
    /// 
    /// </summary>
    public override string Value { get; }
    /// <summary>
    /// 
    /// </summary>
    public override List<SyntaxItem> DaughterItems { get; set; }

    public override SyntaxItem Clone() {
        return new UnaryOperator(Value, DaughterItems.First());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lexemeType"></param>
    /// <param name="syntaxItem"></param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemeType"/> or <paramref name="syntaxItem"/> is null.</exception>
    public UnaryOperator(string lexemeType, SyntaxItem syntaxItem) {
        Value = lexemeType ?? throw new ArgumentNullException(nameof(lexemeType));      //Ensure that the lexemeType is not null
        DaughterItems = new List<SyntaxItem> { syntaxItem } ?? throw new ArgumentNullException(nameof(syntaxItem)); //Ensure that the syntaxItem is not null
    }

    public override uint GetCost() {
        return 1 + DaughterItems.Aggregate<SyntaxItem, uint>(0, (current, daughterItem) => current + daughterItem.GetCost());
    }

    public override string ToString() {
        return $"{Value}{DaughterItems.First()}";  //Outputs in the format [Value, SyntaxItem]
    }

    public override bool Equals(SyntaxItem? other) {
        return other is UnaryOperator otherUnaryOperator            //Check that the other syntaxItem is of the same type
            && Value == otherUnaryOperator.Value                    //Check that the values are equal
            && DaughterItems.First().Equals(otherUnaryOperator.DaughterItems.First());    //Check that the daughter syntaxItems are equal
    }

    public override bool Equals(object? other) {
        return Equals(other as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value, DaughterItems);
    }
}