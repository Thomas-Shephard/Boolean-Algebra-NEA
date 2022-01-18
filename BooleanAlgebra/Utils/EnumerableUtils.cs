namespace BooleanAlgebra.Utils;

public static class EnumerableUtils {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="result"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static bool TryFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, [NotNullWhen(true)] out TSource? result) {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        //Iterate over all of the items in the source.
        foreach (TSource item in source) {
            //If the predicate is false or if the item is null, the item cannot be a match.
            if (!predicate(item) || item is null)
                continue;
            //If the predicate is true and the item is not null, the item is a match.
            result = item;
            return true;
        }

        //If the predicate was false for all items, return false as there was no match.
        result = default;
        return false;
    }
    
    public static bool SequenceEqualsIgnoreOrder<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) where TSource : IEquatable<TSource> {
        if (first is null) throw new ArgumentNullException(nameof(first));
        if (second is null) throw new ArgumentNullException(nameof(second));

        Dictionary<TSource, int> firstItemCounts = first.GetItemCounts();
        
        foreach (TSource item in second) {
            bool hasFoundItem = false;
            KeyValuePair<TSource, int> itemMatch = firstItemCounts.FirstOrDefault(x => x.Key.Equals(item) && (hasFoundItem = true));
            if (!hasFoundItem)
                return false;
            firstItemCounts[itemMatch.Key]--;
        }
        
        return firstItemCounts.Values.All(itemCount => itemCount == 0);
    }
    
    public static Dictionary<TSource, int> GetItemCounts<TSource>(this IEnumerable<TSource> source) where TSource : IEquatable<TSource> {
        if (source is null) throw new ArgumentNullException(nameof(source));

        Dictionary<TSource, int> itemCounts = new();
        foreach (TSource item in source) {
            bool hasFoundItem = false;
            KeyValuePair<TSource, int> itemMatch = itemCounts.FirstOrDefault(x => x.Key.Equals(item) && (hasFoundItem = true));

            if (hasFoundItem) {
                itemCounts[itemMatch.Key]++;
            } else {
                itemCounts.Add(item, 1);
            }
        }

        return itemCounts;
    }
}