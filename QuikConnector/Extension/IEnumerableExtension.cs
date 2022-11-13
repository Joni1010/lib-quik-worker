using System;
using System.Collections;

public static class IEnumerableExtension
{
    /// <summary>
    /// Перебор коллекции
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable source, Action<T> action)
    {
        if (source.IsNull()) return;
        if (!action.IsNull())
        {
            foreach (T el in source) action(el);
        }
    }
}
