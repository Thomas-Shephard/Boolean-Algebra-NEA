namespace BooleanAlgebra.Utils;

public static class EnumerableUtils {
    //Wanted to do this https://stackoverflow.com/a/3670089/14619583
    public static bool SequenceEqualsIgnoreOrder<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) where TSource : IEquatable<TSource> {
        if (first is null) throw new ArgumentNullException(nameof(first));
        if (second is null) throw new ArgumentNullException(nameof(second));

        Dictionary<TSource, int> itemCounts = first.GetItemCounts();
        foreach (TSource item in second) {
            bool hasFoundItem = false;
            KeyValuePair<TSource, int> itemMatch = itemCounts.FirstOrDefault(x => x.Key.Equals(item) && (hasFoundItem = true));
            if (!hasFoundItem)
                return false;
            itemCounts[itemMatch.Key]--;
        }
        
        return itemCounts.Values.All(itemCount => itemCount == 0);
    }
    
    public static Dictionary<TSource, int> GetItemCounts<TSource>(this IEnumerable<TSource> source) where TSource : IEquatable<TSource> {
        if (source is null) throw new ArgumentNullException(nameof(source));

        Dictionary<TSource, int> itemCounts = new();
        foreach (TSource item in source) {
            IEnumerable<KeyValuePair<TSource, int>> x = itemCounts.Where(x => x.Key.Equals(item)).ToArray();

            if (x.Any())
                itemCounts[x.First().Key]++;
            else {
                itemCounts.Add(item, 1);
            }
        }

        return itemCounts;
    }
}