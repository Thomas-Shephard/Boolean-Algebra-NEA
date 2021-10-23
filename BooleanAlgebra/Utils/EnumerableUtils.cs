namespace BooleanAlgebra.Utils;
public static class EnumerableUtils {
    //Wanted to do this https://stackoverflow.com/a/3670089/14619583
    //Made it better
    //More efficient and robust
    public static bool SequenceEqualsIgnoreOrder<T>(this IEnumerable<T> list1, IEnumerable<T> list2) where T : IEquatable<T> {
        Dictionary<T, int> itemCounts = new();
        foreach (T item in list1) {
            itemCounts.TryGetValue(item, out int currentCount);
            itemCounts[item] = currentCount + 1;
        }
        foreach (T item in list2) {
            if (!itemCounts.TryGetValue(item, out int currentCount))
                return false;
            itemCounts[item] = currentCount - 1;
        }
        return itemCounts.Values.All(itemCount => itemCount == 0);
    }
}