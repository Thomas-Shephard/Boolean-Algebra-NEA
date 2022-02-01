namespace BooleanAlgebra.Utils.SyntaxItem;
/// <summary>
/// Holds the information about the number of times a specified syntax item occurred in a given collection of syntax items.
/// </summary>
/// <param name="SyntaxItem">The syntax item that is being counted.</param>
public record SyntaxItemCountPair(ISyntaxItem SyntaxItem) {
    /// <summary>
    /// The number of times the syntax item has been used.
    /// The default value is 1 as during the counting process, the count is incremented for each occurrence of the syntax item.
    /// </summary>
    public int Count { get; set; } = 1;
}