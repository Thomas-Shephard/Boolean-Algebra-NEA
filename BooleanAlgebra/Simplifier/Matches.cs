namespace BooleanAlgebra.Simplifier; 
/// <summary>
/// 
/// </summary>
public class Matches {
    /// <summary>
    /// 
    /// </summary>
    public List<DirectSubstitute> DirectSubstitutes { get;  }
    /// <summary>
    /// 
    /// </summary>
    public List<RepeatingSubstitute> RepeatingSubstitutes { get;  }
    
    /// <summary>
    /// 
    /// </summary>
    public Matches() {
        DirectSubstitutes = new List<DirectSubstitute>();
        RepeatingSubstitutes = new List<RepeatingSubstitute>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="directSubstitutes"></param>
    /// <param name="repeatingSubstitutes"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Matches(List<DirectSubstitute> directSubstitutes, List<RepeatingSubstitute> repeatingSubstitutes) {
        DirectSubstitutes = directSubstitutes ?? throw new ArgumentNullException(nameof(directSubstitutes));
        RepeatingSubstitutes = repeatingSubstitutes ?? throw new ArgumentNullException(nameof(repeatingSubstitutes));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Matches Clone() {
        return new Matches(DirectSubstitutes.ToList(), RepeatingSubstitutes.ToList());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genericOperand"></param>
    /// <param name="substitute"></param>
    /// <returns></returns>
    public bool TryGetDirectSubstituteFromGenericOperand(GenericOperand genericOperand, [NotNullWhen(true)] out ISyntaxItem? substitute) {
        if (genericOperand is null)
            throw new ArgumentNullException(nameof(genericOperand));
        substitute = DirectSubstitutes.FirstOrDefault(directSubstitute => directSubstitute.SubstitutableSyntaxItem.Equals(genericOperand))?.Substitution;
        return substitute is not null;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="genericOperand"></param>
    /// <param name="substitutes"></param>
    /// <returns></returns>
    public bool TryGetRepeatingSubstituteFromGenericOperand(GenericOperand genericOperand, [NotNullWhen(true)] out List<ISyntaxItem>? substitutes) {
        if (genericOperand is null)
            throw new ArgumentNullException(nameof(genericOperand));
        substitutes = RepeatingSubstitutes.FirstOrDefault(repeatingSubstitute => repeatingSubstitute.SubstitutableSyntaxItem.Equals(genericOperand))?.Substitutions;
        return substitutes is not null;
    }
}

/// <summary>
/// 
/// </summary>
public class DirectSubstitute {
    /// <summary>
    /// 
    /// </summary>
    public GenericOperand SubstitutableSyntaxItem { get; }
    /// <summary>
    /// 
    /// </summary>
    public ISyntaxItem Substitution { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="substitutableSyntaxItem"></param>
    /// <param name="substitution"></param>
    public DirectSubstitute(GenericOperand substitutableSyntaxItem, ISyntaxItem substitution) {
        SubstitutableSyntaxItem = substitutableSyntaxItem ?? throw new ArgumentNullException(nameof(substitutableSyntaxItem));
        Substitution = substitution ?? throw new ArgumentNullException(nameof(substitution));
    }
}

/// <summary>
/// 
/// </summary>
public class RepeatingSubstitute {
    /// <summary>
    /// 
    /// </summary>
    public GenericOperand SubstitutableSyntaxItem { get; }
    /// <summary>
    /// 
    /// </summary>
    public List<ISyntaxItem> Substitutions { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="substitutableSyntaxItem"></param>
    /// <param name="substitutions"></param>
    public RepeatingSubstitute(GenericOperand substitutableSyntaxItem, List<ISyntaxItem> substitutions) {
        SubstitutableSyntaxItem = substitutableSyntaxItem ?? throw new ArgumentNullException(nameof(substitutableSyntaxItem));
        Substitutions = substitutions ?? throw new ArgumentNullException(nameof(substitutions));
    }
}