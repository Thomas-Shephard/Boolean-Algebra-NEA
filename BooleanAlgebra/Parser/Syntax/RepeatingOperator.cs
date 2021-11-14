namespace BooleanAlgebra.Parser.Syntax;
/// <summary>
/// 
/// </summary>
public class RepeatingOperator : SyntaxItem {
    public override uint GetCost() {
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public override string Value { get; }
    /// <summary>
    /// 
    /// </summary>
    public override List<SyntaxItem> DaughterItems { get; set; }

    public SyntaxItem DaughterItem => DaughterItems[0];

    public override SyntaxItem Clone() {
        return new RepeatingOperator(Value, DaughterItem);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lexemeType"></param>
    /// <param name="syntaxItem"></param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="lexemeType"/> or <paramref name="syntaxItem"/> is null.</exception>
    public RepeatingOperator(string lexemeType, SyntaxItem syntaxItem) {
        Value = lexemeType ?? throw new ArgumentNullException(nameof(lexemeType));      //Ensure that the lexemeType is not null
        DaughterItems = new List<SyntaxItem>() { syntaxItem } ?? throw new ArgumentNullException(nameof(syntaxItem)); //Ensure that the syntaxItem is not null
    }

    

    public override string ToString() {
        return $"[{DaughterItems.First()}]";  //Outputs in the format [Value, SyntaxItem]
    }

    public override bool Equals(SyntaxItem? other) {
        return other is RepeatingOperator otherRepeatingOperator            //Check that the other syntaxItem is of the same type
               && Value == otherRepeatingOperator.Value                    //Check that the values are equal
               && DaughterItems.First().Equals(otherRepeatingOperator.DaughterItems.First());    //Check that the daughter syntaxItems are equal
    }

    public override bool Equals(object? other) {
        return Equals(other as SyntaxItem);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Value, DaughterItems);
    }
}