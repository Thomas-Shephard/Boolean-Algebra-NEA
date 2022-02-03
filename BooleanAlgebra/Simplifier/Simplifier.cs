namespace BooleanAlgebra.Simplifier;
/// <summary>
/// Provides a method to simplify a given syntax item into a simplified form and produce the respective simplification steps.
/// </summary>
public class Simplifier {
    /// <summary>
    /// The message that is associated with the original syntax item in the simplification process.
    /// This is the string that will describe the initial boolean expression.
    /// </summary>
    private const string StartMessage = "Initial boolean expression";
    /// <summary>
    /// The original syntax item that is being simplified.
    /// </summary>
    private ISyntaxItem OriginalSyntaxItem { get; }

    /// <summary>
    /// Initialises a new simplifier that will be able to simplify the given syntax item and produce the respective simplification steps.
    /// </summary>
    /// <param name="originalSyntaxItem">The original syntax item that will be simplified.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="originalSyntaxItem"/> is null.</exception>
    public Simplifier(ISyntaxItem originalSyntaxItem) {
        OriginalSyntaxItem = originalSyntaxItem ?? throw new ArgumentNullException(nameof(originalSyntaxItem));
    }

    /// <summary>
    /// Produces a list of steps that will simplify the original syntax item to a simplified form of itself.
    /// </summary>
    /// <returns>A list of steps that will simplify the original syntax item to a simplified form of itself.</returns>
    public List<SimplificationRulePair> Simplify() {
        //Create an apex node from the original syntax item so that it can be simplified.
        //Use the start message as the message for the apex node.
        SimplificationItem simplifierTree = new(OriginalSyntaxItem, StartMessage);
        simplifierTree.Simplify();
        //Create a new list of steps to store the process by which the original syntax item was simplified.
        List<SimplificationRulePair> simplificationSteps = new();
        SimplificationItem.GetSmallestCostSyntaxItemWithSmallestDepth(simplifierTree).GetSimplificationSteps(simplificationSteps);
        return simplificationSteps;
    }
}