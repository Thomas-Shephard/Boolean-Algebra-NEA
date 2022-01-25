namespace BooleanAlgebra.Simplifier; 

public class Matches {
    public Dictionary<GenericOperand, ISyntaxItem> DirectSubstitutes { get; }
    public Dictionary<GenericOperand, List<ISyntaxItem>> RepeatingSubstitutes { get; }

    public Matches() {
        DirectSubstitutes = new Dictionary<GenericOperand, ISyntaxItem>();
        RepeatingSubstitutes = new Dictionary<GenericOperand, List<ISyntaxItem>>();
    }

    private Matches(Dictionary<GenericOperand, ISyntaxItem> directSubstitutes, Dictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes) {
        DirectSubstitutes = directSubstitutes;
        RepeatingSubstitutes = repeatingSubstitutes;
    }

    public Matches Clone() {
        Dictionary<GenericOperand, ISyntaxItem> directSubstitutesCopy =
            DirectSubstitutes.ToDictionary(x => x.Key, x => x.Value);
        Dictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutesCopy =
            RepeatingSubstitutes.ToDictionary(x => x.Key, x => x.Value);
        return new Matches(directSubstitutesCopy, repeatingSubstitutesCopy);
    }
}