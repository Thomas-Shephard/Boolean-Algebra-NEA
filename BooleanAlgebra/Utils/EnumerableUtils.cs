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
            IEnumerable<KeyValuePair<TSource, int>> x = itemCounts.Where(x => x.Key.Equals(item)).ToArray();

            if (x.Count() != 1)
                return false;
            
            itemCounts[x.First().Key]--;
            
            /*if(!)
            
            if (!itemCounts.Where(x => x.Value > 0).(x => x.Key.Equals(item)))
                return false;
            
            /*if (!itemCounts.ContainsKey(item))
                return false;*/
            
            /*if (!itemCounts.Any(x => x.Key.Equals(item)))
                return false;*/
           /* if(itemCounts[item] == 0)
                return false;*/
           // itemCounts[item]--;
            
          /*  if (!itemCounts.TryGetValue(item, out int currentCount))
                return false;
            itemCounts[item] = currentCount - 1;*/
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
            
            //itemCounts.TryGetValue(item, out int currentCount);
            //itemCounts[item] = currentCount + 1;
        }

        return itemCounts;
    }
}