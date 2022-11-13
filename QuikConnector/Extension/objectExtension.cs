public static class objectExtension
{
    /// <summary> Функция проверяет пустой объект или нет. </summary>
    /// <param name="self">Объект</param>
    /// <returns>true - если объект пуст</returns>
    public static bool Empty(this object self)
    {
        if (self == null)
        {
            return true;
        }
        else if (self is string && string.IsNullOrEmpty((string)self))
        {
            return true;
        }
        else if (self is int && (int)self == 0)
        {
            return true;
        }
        else if (self is long && (long)self == 0)
        {
            return true;
        }
        else if (self is decimal && (decimal)self == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary> Проверяет является ли объект null </summary>
    /// <param name="self"></param>
    /// <returns> true - если объект равен null</returns>
    public static bool IsNull(this object self)
    {
        if (self == null)
        {
            return true;
        }
        return false;
    }

    /// <summary> Проверяет является ли объект null </summary>
    /// <param name="self"></param>
    /// <returns> true - если объект равен null</returns>
    public static bool NotIsNull(this object self)
    {
        return !self.IsNull();
    }

    public static T Clone<T>(this T obj)
    {
        var inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        return (T)inst?.Invoke(obj, null);
    }
}