namespace BooleanAlgebra.Simplifier.Logic; 
/// <summary>
/// The traversal order by which the syntax tree is traversed to apply the simplification rules.
/// The order is important because the simplification rule that matches part of the syntax tree is applied first.
/// </summary>
public enum SimplificationRuleTraversalOrder {
    /// <summary>
    /// The inside out option means that the most nested nodes in the syntax tree will be compared against first.
    /// </summary>
    INSIDE_OUT,
    /// <summary>
    /// The outside in option means that the least nested nodes in the syntax tree will be compared against first.
    /// </summary>
    OUTSIDE_IN
}