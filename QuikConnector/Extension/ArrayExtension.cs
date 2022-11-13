using System;

public static class ArrayExtension
{
    /// <summary>
    /// Перебор массива
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this Array source, Action<T> action)
    {
        if (source.IsNull() || source.Length == 0) return;
        if (!action.IsNull())
        {
            foreach (T el in source) action(el);
        }
    }
}