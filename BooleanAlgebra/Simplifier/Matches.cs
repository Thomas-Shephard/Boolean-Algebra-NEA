namespace BooleanAlgebra.Simplifier; 

public class Matches {
    public List<DirectSubstitute> DirectSubstitutes { get;  }
    public List<RepeatingSubstitute> RepeatingSubstitutes { get;  }
    
    public Matches() {
        DirectSubstitutes = new List<DirectSubstitute>();
        RepeatingSubstitutes = new List<RepeatingSubstitute>();
    }

    public Matches(List<DirectSubstitute> directSubstitutes, List<RepeatingSubstitute> repeatingSubstitutes) {
        DirectSubstitutes = directSubstitutes;
        RepeatingSubstitutes = repeatingSubstitutes;
    }

    public Matches Clone() {
        return new Matches(DirectSubstitutes.ToList(), RepeatingSubstitutes.ToList());
    }

    public bool TryGetDirectSubstituteFromGenericOperand(GenericOperand genericOperand, [NotNullWhen(true)] out ISyntaxItem? substitute) {
        substitute = DirectSubstitutes.FirstOrDefault(directSubstitute => directSubstitute.SubstitutableSyntaxItem.Equals(genericOperand))?.Substitution;
        return substitute is not null;
    }
    
    public bool TryGetRepeatingSubstituteFromGenericOperand(GenericOperand genericOperand, [NotNullWhen(true)] out List<ISyntaxItem>? substitutes) {
        substitutes = RepeatingSubstitutes.FirstOrDefault(repeatingSubstitute => repeatingSubstitute.SubstitutableSyntaxItem.Equals(genericOperand))?.Substitutions;
        return substitutes is not null;
    }
    
    

 /*   private Matches(Dictionary<GenericOperand, ISyntaxItem> directSubstitutes, Dictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutes) {
        DirectSubstitutes = directSubstitutes;
        RepeatingSubstitutes = repeatingSubstitutes;
    }

    public Matches Clone() {
        Dictionary<GenericOperand, ISyntaxItem> directSubstitutesCopy =
            DirectSubstitutes.ToDictionary(x => x.Key, x => x.Value);
        Dictionary<GenericOperand, List<ISyntaxItem>> repeatingSubstitutesCopy =
            RepeatingSubstitutes.ToDictionary(x => x.Key, x => x.Value);
        return new Matches(directSubstitutesCopy, repeatingSubstitutesCopy);
    } */
}

public class DirectSubstitute {
    public GenericOperand SubstitutableSyntaxItem { get; }
    public ISyntaxItem Substitution { get; }

    public DirectSubstitute(GenericOperand substitutableSyntaxItem, ISyntaxItem substitution) {
        SubstitutableSyntaxItem = substitutableSyntaxItem;
        Substitution = substitution;
    }
}

public class RepeatingSubstitute {
    public GenericOperand SubstitutableSyntaxItem { get; }
    public List<ISyntaxItem> Substitutions { get; }

    public RepeatingSubstitute(GenericOperand substitutableSyntaxItem, List<ISyntaxItem> substitutions) {
        SubstitutableSyntaxItem = substitutableSyntaxItem;
        Substitutions = substitutions;
    }
}