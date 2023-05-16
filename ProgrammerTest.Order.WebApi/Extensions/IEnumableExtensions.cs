namespace System.Collections.Generic;

public static class IEnumableExtensions
{
    /// <summary>
    ///     遍历IEnumable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source) action(item);
    }

    /// <summary>
    ///     遍历IEnumable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task ForEachAsync<T>(this IEnumerable<T> source, Action<T> action)
    {
        return Task.Run(() => Parallel.ForEach(source, action));
    }

    /// <summary>
    ///     按字段去重
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var hash = new HashSet<TKey>();
        return source.Where(p => hash.Add(keySelector(p)));
    }


    public static string ToString<T>(this IEnumerable<T>? source, string separator)
    {
        return source != null && source.Any() ? string.Join(separator, source) : string.Empty;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source != null && source.Any();
    }
}