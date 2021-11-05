using BooleanAlgebra.Simplification.Logic;

namespace BooleanAlgebra.Simplification;
public class Simplification {
    public SyntaxItem StartSyntaxItem { get; }
    public List<Tuple<SyntaxItem, string?>> Simplifications { get; }

    public Simplification(SyntaxItem startSyntaxItem) {
        StartSyntaxItem = startSyntaxItem;
        Simplifications = new List<Tuple<SyntaxItem, string?>>();

        Tuple<SyntaxItem, string>? simplification = new(startSyntaxItem, "");
        bool didSimplify;
        do {
            if (!string.IsNullOrEmpty(simplification?.Item2))
                Simplifications.Add(simplification);
            didSimplify = AttemptSimplification.TrySimplify(simplification.Item1, out simplification);
        } while (didSimplify);
    }

    public override string ToString() {
        StringBuilder returnValue = new($"[Start ----------, {StartSyntaxItem}]");
        for (int i = 0; i < Simplifications.Count; i++) {
            returnValue.Append($"{Environment.NewLine}[Simplification {i + 1}, {Simplifications[i].Item1}, {Simplifications[i].Item2}]");
        }

        return returnValue.ToString();
    }
}