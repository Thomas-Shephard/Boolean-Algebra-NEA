namespace BooleanAlgebra.Utils.SyntaxItem; 
/// <summary>
/// Holds the information about the number of times a specified syntax item occurred in a given collection of syntax items.
/// </summary>
public class SyntaxItemCountPair {
    /// <summary>
    /// The syntax item that is being counted.
    /// </summary>
    public ISyntaxItem SyntaxItem { get; }
    /// <summary>
    /// The number of times the syntax item has been used.
    /// The default value is 1 as during the counting process, the count is incremented for each occurrence of the syntax item.
    /// </summary>
    public int Count { get; set; } = 1;
    
    /// <summary>
    /// Instantiates a new syntax item count pair with the specified syntax item and a default count of 1.
    /// </summary>
    /// <param name="syntaxItem">The syntax item that is being counted</param>
    public SyntaxItemCountPair(ISyntaxItem syntaxItem) {
        SyntaxItem = syntaxItem;
    }
    
    /// <summary>
    /// Allows the syntax item count pair to be deconstructed into its constituent parts.
    /// </summary>
    /// <param name="syntaxItem">The syntax item that has been counted.</param>
    /// <param name="count">The number of times the syntax item occurred.</param>
    public void Deconstruct(out ISyntaxItem syntaxItem, out int count) {
        syntaxItem = SyntaxItem;
        count = Count;
    }
}