namespace BooleanAlgebra.Utils;
/// <summary>
/// Provides a set of extension methods for working with <see cref="System.Collections.Generic.IEnumerable{T}"/>.
/// </summary>
public static class EnumerableUtils {
    /// <summary>
    /// Finds the first element in a collection that satisfied a given predicate.
    /// If an element is found, returns true and the matching element is provided by the result parameter.
    /// If no such element is found, returns false and the default value is provided by the result parameter.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The condition that must be met by the item within the collection.</param>
    /// <param name="result">The first item within the collection that met the predicate or the default value if no item within the collection met the predicate.</param>
    /// <typeparam name="TSource">The type that the collection's items are of.</typeparam>
    /// <returns>True when an item in the collection met the predicate.</returns>
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
    
    /// <summary>
    /// Adds an element to a collection a given number of times.
    /// </summary>
    /// <param name="source">The collection to add the elements to.</param>
    /// <param name="item">The item to add to the collection.</param>
    /// <param name="count">The number of times to add the item to the collection.</param>
    /// <typeparam name="TSource">The type that the collection's items are of.</typeparam>
    public static void AddRepeated<TSource>(this ICollection<TSource> source, TSource item, int count) {
        for (int i = 0; i < count; i++)
            source.Add(item);
    }
}