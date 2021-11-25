namespace BooleanAlgebra.Simplifier; 

public class Matches {
    public Dictionary<GenericOperand, SyntaxItem> DirectSubstitutes { get; }
    public Dictionary<GenericOperand, List<SyntaxItem>> RepeatingSubstitutes { get; }

    public Matches() {
        DirectSubstitutes = new Dictionary<GenericOperand, SyntaxItem>();
        RepeatingSubstitutes = new Dictionary<GenericOperand, List<SyntaxItem>>();
    }

    private Matches(Dictionary<GenericOperand, SyntaxItem> directSubstitutes, Dictionary<GenericOperand, List<SyntaxItem>> repeatingSubstitutes) {
        DirectSubstitutes = directSubstitutes;
        RepeatingSubstitutes = repeatingSubstitutes;
    }

    public Matches Clone() {
        Dictionary<GenericOperand, SyntaxItem> directSubstitutesCopy =
            DirectSubstitutes.ToDictionary(x => x.Key, x => x.Value);
        Dictionary<GenericOperand, List<SyntaxItem>> repeatingSubstitutesCopy =
            RepeatingSubstitutes.ToDictionary(x => x.Key, x => x.Value);
        return new Matches(directSubstitutesCopy, repeatingSubstitutesCopy);
    }
}