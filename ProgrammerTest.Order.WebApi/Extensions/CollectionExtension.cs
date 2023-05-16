using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic;

public static class CollectionExtension
{
    public static void AddIf<T>([NotNull] this ICollection<T> @this, Func<T, bool> predicate, T item)
    {
        if (@this.IsReadOnly)
            throw new InvalidOperationException($"{nameof(@this)} is readonly");

        if (predicate(item))
            @this.Add(@item);
    }

    public static bool ContainsAll<T>([NotNull] this ICollection<T> @this, params T[] values)
    {
        foreach (var item in values)
        {
            if (!@this.Contains(item))
                return false;
        }

        return true;
    }

    public static bool ContainsAny<T>([NotNull] this ICollection<T> @this, params T[] values)
    {
        foreach (var item in values)
        {
            if (@this.Contains(item))
                return true;
        }

        return false;
    }


    public static bool IsNullOrEmpty<T>(this ICollection<T> @this)
    {
        return @this == null || @this.Count == 0;
    }

    public static bool IsNotNullOrEmpty<T>(this ICollection<T> @this)
    {
        return @this != null && @this.Any();
    }
}
