namespace BooleanAlgebra.Utils.SyntaxItem; 
/// <summary>
/// Provides a set of methods that allow for the checking of equality between collections of syntax items.
/// </summary>
public static class SyntaxItemEqualityUtils {
    /// <summary>
    /// Allows two collections to be compared for equality by comparing the contents of each collection.
    /// This method calculates equality independently of the orders of the items in the collections.
    /// </summary>
    /// <param name="first">The first collection of syntax items to be compared to.</param>
    /// <param name="second">The second collection of syntax items to be compared to.</param>
    /// <returns>True if the contents of the first and second enumerable contain the same syntax items but allows them to be in a different order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="first"/> or <paramref name="second"/> is null.</exception>
    /// <example>Means that A, B, C, D is equal to B, C, D, A.</example>
    public static bool SyntaxItemsEqualIgnoreOrder(this IEnumerable<ISyntaxItem> first, IEnumerable<ISyntaxItem> second) {
        if (first is null) throw new ArgumentNullException(nameof(first));
        if (second is null) throw new ArgumentNullException(nameof(second));

        //Get the counts of all the items in the first collection of syntax items.
        List<SyntaxItemCountPair> firstSyntaxItemCounts = GetSyntaxItemCounts(first);
        
        //Iterate over the second collection of syntax items to ensure that the counts of all the items in the first collection are the same.
        foreach (ISyntaxItem syntaxItem in second) {
            //Attempt to find the current syntax item within the list of the counts for the first collection.
            //If the item is not found, current syntax item count will be null.
            SyntaxItemCountPair? currentSyntaxItemCount = firstSyntaxItemCounts.FirstOrDefault(syntaxItemCountPair => syntaxItemCountPair.SyntaxItem.Equals(syntaxItem));
            if(currentSyntaxItemCount is null || currentSyntaxItemCount.Count == 0)
                //If the current syntax item is null, they cannot be equal as the first collection does not contain the current syntax item.
                //If the current syntax item count is 0, the first collection did contain the current syntax item but has already been used all of the available times.
                return false;
            //Decrement the current syntax item count by 1 to indicate that the current syntax item has been used.
            currentSyntaxItemCount.Count--;
        }
        
        //Only return true if all of the counts for the first collection are 0.
        //This means that all of the items in the first collection have been used.
        return firstSyntaxItemCounts.All(itemCount => itemCount.Count == 0);
    }

    /// <summary>
    /// Allows for the number of times each distinct syntax item appears in a collection to be counted.
    /// Distinctness is determined by the equality of the syntax items.
    /// </summary>
    /// <param name="source">The collection of syntax items that a count is desired from.</param>
    /// <returns>A list of syntax item count pairs that represent the number of times each distinct syntax item occurred in the source collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static List<SyntaxItemCountPair> GetSyntaxItemCounts(this IEnumerable<ISyntaxItem> source) {
        if (source is null) throw new ArgumentNullException(nameof(source));
        List<SyntaxItemCountPair> syntaxItemCounts = new();
        
        //Iterate over all of the syntax items in the source to get the counts.
        foreach (ISyntaxItem syntaxItem in source) {
            //Attempt to find the current syntax item within the list of previously added syntax items.
            //If the syntax item is not found, the current syntax item count will be null.
            SyntaxItemCountPair? currentSyntaxItemCount = syntaxItemCounts.FirstOrDefault(syntaxItemCountPair => syntaxItemCountPair.SyntaxItem.Equals(syntaxItem));
            if (currentSyntaxItemCount is null) {
                //If the current syntax item is not in the list, add it with the default count of 1.
                syntaxItemCounts.Add(new SyntaxItemCountPair(syntaxItem));
            } else {
                //If the current syntax item is already in the list, increment the count by 1.
                currentSyntaxItemCount.Count++;
            }
        }

        return syntaxItemCounts;
    }
}