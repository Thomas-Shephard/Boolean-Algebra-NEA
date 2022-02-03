namespace BooleanAlgebra.Simplifier;
/// <summary>
/// Holds information about a single simplification of a boolean expression and the rule that was used to simplify it.
/// </summary>
/// <param name="SimplifiedSyntaxItem">A simplified form of the original boolean expression.</param>
/// <param name="Rule">The rule that what used to allow the simplification to take place.</param>
public record SimplificationRulePair(ISyntaxItem SimplifiedSyntaxItem, string Rule);