namespace BooleanAlgebra.Simplifier; 
/// <summary>
/// The order by which the simplifier should process the expression.
/// </summary>
public enum SimplificationTraversalOrder {
    /// <summary>
    /// The inside out option means that the most nested expression will be processed first.
    /// </summary>
    INSIDE_OUT,
    /// <summary>
    /// The outside in option means that the least nested expression will be processed first.
    /// </summary>
    OUTSIDE_IN
}