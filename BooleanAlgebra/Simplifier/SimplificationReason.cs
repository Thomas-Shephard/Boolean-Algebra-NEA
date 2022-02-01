namespace BooleanAlgebra.Simplifier;
/// <summary>
/// Holds information about a simplification and the reason why it was applied.
/// </summary>
/// <param name="SimplifiedSyntaxItem">The simplified form of a syntax item.</param>
/// <param name="Reason">The reason by which a simplification can take place.</param>
public record SimplificationReason(ISyntaxItem SimplifiedSyntaxItem, string Reason);