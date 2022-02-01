namespace BooleanAlgebra.Simplifier.Logic; 
/// <summary>
/// Holds all of the substitutes for the generic operands found within the matching process.
/// </summary>
public class Matches {
    /// <summary>
    /// All of the direct substitutes that are found during the matching process.
    /// </summary>
    public List<DirectSubstitute> DirectSubstitutes { get; }
    /// <summary>
    /// All of the repeating substitutes that are found during the matching process.
    /// </summary>
    public List<RepeatingSubstitute> RepeatingSubstitutes { get; }
    
    /// <summary>
    /// Initialises a new matches with an empty list of direct substitutes and an empty list of repeating substitutes.
    /// </summary>
    public Matches() {
        DirectSubstitutes = new List<DirectSubstitute>();
        RepeatingSubstitutes = new List<RepeatingSubstitute>();
    }

    /// <summary>
    /// Initialises a new matches with the given list of direct substitutes and given list of repeating substitutes.
    /// </summary>
    /// <param name="directSubstitutes">All of the direct substitutes that are found during the matching process.</param>
    /// <param name="repeatingSubstitutes">All of the repeating substitutes that are found during the matching process.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="directSubstitutes"/> or <paramref name="repeatingSubstitutes"/> is null.</exception>
    public Matches(List<DirectSubstitute> directSubstitutes, List<RepeatingSubstitute> repeatingSubstitutes) {
        DirectSubstitutes = directSubstitutes ?? throw new ArgumentNullException(nameof(directSubstitutes));
        RepeatingSubstitutes = repeatingSubstitutes ?? throw new ArgumentNullException(nameof(repeatingSubstitutes));
    }

    /// <summary>
    /// Creates a new matches object with a shallow copy of the direct substitutes and repeating substitutes.
    /// </summary>
    /// <returns>A new matches object with a shallow copy of the direct substitutes and repeating substitutes.</returns>
    public Matches Clone() {
        return new Matches(DirectSubstitutes.ToList(), RepeatingSubstitutes.ToList());
    }

    /// <summary>
    /// Attempts to find the direct substitute for a given generic operand.
    /// If it is found, the method returns true and the substitute is passed out of the method.
    /// If it is not found, the method returns false and the substitute is passed out of the method as null.
    /// </summary>
    /// <param name="genericOperand">The generic operand that a direct substitute should be found from.</param>
    /// <param name="substitute">The substitute for the generic operand.</param>
    /// <returns>True if a direct substitute exists for the given generic operand.</returns>
    public bool TryGetDirectSubstituteFromGenericOperand(GenericOperand genericOperand, [NotNullWhen(true)] out ISyntaxItem? substitute) {
        if (genericOperand is null)
            throw new ArgumentNullException(nameof(genericOperand));
        substitute = DirectSubstitutes.FirstOrDefault(directSubstitute => directSubstitute.SubstitutableSyntaxItem.Equals(genericOperand))?.Substitution;
        return substitute is not null;
    }
    
    /// <summary>
    /// Attempts to find the repeating substitute for a given generic operand.
    /// If it is found, the method returns true and all the substitutes for the generic operand are passed out of the method.
    /// If it is not found, the method returns false and substitutes for the generic operand is set to null.
    /// </summary>
    /// <param name="genericOperand">The generic operand that a repeating substitute should be found from.</param>
    /// <param name="substitutes">All of the substitutes for the given generic operand.</param>
    /// <returns>True if a repeating substitute exists for the given generic operand.</returns>
    public bool TryGetRepeatingSubstituteFromGenericOperand(GenericOperand genericOperand, [NotNullWhen(true)] out List<ISyntaxItem>? substitutes) {
        if (genericOperand is null)
            throw new ArgumentNullException(nameof(genericOperand));
        substitutes = RepeatingSubstitutes.FirstOrDefault(repeatingSubstitute => repeatingSubstitute.SubstitutableSyntaxItem.Equals(genericOperand))?.Substitutions;
        return substitutes is not null;
    }
}

/// <summary>
/// Holds information about a direct substitute including the generic operand to be substituted and its associated substitution.
/// </summary>
public class DirectSubstitute {
    /// <summary>
    /// The generic operand that is to be substituted.
    /// </summary>
    public GenericOperand SubstitutableSyntaxItem { get; }
    /// <summary>
    /// The substitute for the generic operand.
    /// </summary>
    public ISyntaxItem Substitution { get; }

    /// <summary>
    /// Initialises a new direct substitute with the generic operand to be substituted and its associated substitution.
    /// </summary>
    /// <param name="substitutableSyntaxItem">The generic operand that is to be substituted.</param>
    /// <param name="substitution">The substitute for the generic operand.</param>
    public DirectSubstitute(GenericOperand substitutableSyntaxItem, ISyntaxItem substitution) {
        SubstitutableSyntaxItem = substitutableSyntaxItem ?? throw new ArgumentNullException(nameof(substitutableSyntaxItem));
        Substitution = substitution ?? throw new ArgumentNullException(nameof(substitution));
    }
}

/// <summary>
/// Holds information about a repeating substitute including the generic operand to be substituted and all the associated substitutes.
/// </summary>
public class RepeatingSubstitute {
    /// <summary>
    /// The generic operand that is to be substituted.
    /// </summary>
    public GenericOperand SubstitutableSyntaxItem { get; }
    /// <summary>
    /// All of the substitutes for the generic operand.
    /// </summary>
    public List<ISyntaxItem> Substitutions { get; }

    /// <summary>
    /// Initialises a new repeating substitute with the generic operand to be substituted and all the associated substitutes.
    /// </summary>
    /// <param name="substitutableSyntaxItem">The generic operand that is to be substituted.</param>
    /// <param name="substitutions">All of the substitutes for the generic operand.</param>
    public RepeatingSubstitute(GenericOperand substitutableSyntaxItem, List<ISyntaxItem> substitutions) {
        SubstitutableSyntaxItem = substitutableSyntaxItem ?? throw new ArgumentNullException(nameof(substitutableSyntaxItem));
        Substitutions = substitutions ?? throw new ArgumentNullException(nameof(substitutions));
    }
}