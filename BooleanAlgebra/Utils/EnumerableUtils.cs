namespace BooleanAlgebra.Utils;
public static class EnumerableUtils {
    //Wanted to do this https://stackoverflow.com/a/3670089/14619583
    //Made it better
    //More efficient and robust
    public static bool SequenceEqualsIgnoreOrder<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) where TSource : IEquatable<TSource> {
        if (first is null) throw new ArgumentNullException(nameof(first));
        if (second is null) throw new ArgumentNullException(nameof(second));

        Dictionary<TSource, int> itemCounts = first.GetItemCounts();
        foreach (TSource item in second) {
            if (!itemCounts.TryGetValue(item, out int currentCount))
                return false;
            itemCounts[item] = currentCount - 1;
        }
        
        return itemCounts.Values.All(itemCount => itemCount == 0);
    }
    
    public static Dictionary<TSource, int> GetItemCounts<TSource>(this IEnumerable<TSource> source) where TSource : IEquatable<TSource> {
        if (source is null) throw new ArgumentNullException(nameof(source));
        
        Dictionary<TSource, int> itemCounts = new();
        foreach (TSource item in source) {
            itemCounts.TryGetValue(item, out int currentCount);
            itemCounts[item] = currentCount + 1;
        }

        return itemCounts;
    }
}