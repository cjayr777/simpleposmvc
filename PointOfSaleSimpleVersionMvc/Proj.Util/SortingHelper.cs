using System.Linq.Expressions;

namespace Proj.Util;

public static class SortingHelper
{
    /* ===================== IEnumerable ===================== */

    public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        SortMode sortMode)
    {
        return sortMode == SortMode.desc
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
    }

    public static IOrderedEnumerable<TSource> ThenByWithDirection<TSource, TKey>(
        this IOrderedEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        SortMode sortMode)
    {
        return sortMode == SortMode.desc
            ? source.ThenByDescending(keySelector)
            : source.ThenBy(keySelector);
    }

    /* ===================== IQueryable ===================== */

    public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        SortMode sortMode)
    {
        return sortMode == SortMode.desc
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
    }

    public static IOrderedQueryable<TSource> ThenByWithDirection<TSource, TKey>(
        this IOrderedQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        SortMode sortMode)
    {
        return sortMode == SortMode.desc
            ? source.ThenByDescending(keySelector)
            : source.ThenBy(keySelector);
    }

    public static T SingleOrEmptyObject<T>(this T obj) where T : new()
    {
        return obj ?? new T();
    }

    public static T SingleOrCreate<T>(this T? obj, Func<T> factory)
    {
        return obj ?? factory();
    }

}
