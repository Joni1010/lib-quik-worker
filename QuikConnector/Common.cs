using System;
using System.Collections;
using System.Linq;
using QuikConnector.MarketObjects;
using System.ComponentModel;

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

public static class DateTimeExtension
{
    /// <summary> Конвертирует дату в строку формата YYYYMMDD </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ToString_YYYYMMDD(this DateTime date)
    {
        if (date.Year > 2050) return null;
        if (date.Year < 1900) return null;
        return date.Year.ToString() +
            (date.Month < 10 ? '0' + date.Month.ToString() : date.Month.ToString()) +
            (date.Day < 10 ? '0' + date.Day.ToString() : date.Day.ToString());
    }
}

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

public static class stringExtension
{
    public static decimal ToDecimalE(this string str, int scale = -1)
    {
        str = str.ToLower();
        var lines = new long[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        if (str.Contains("e"))
        {
            var parts = str.Split('e');
            if (parts.Length == 2)
            {
                var value = parts[0].ToDecimal();
                var exp = parts[1].ToInt32();
                var i = exp >= 0 ? exp : exp * -1;
                if (exp >= 0)
                {
                    value = value * lines[i];
                }
                else
                {
                    value = value / lines[i];
                }
                return Math.Round(value, i);
            }
        }
        return str.ToDecimal(scale);
    }
    public static decimal ToDecimal(this string str, int scale = -1)
    {
        if (str.Length == 0) return 0;
        str = str.Replace(" ", "");
        if (str.Contains('.'))
        {
            str = str.Replace(".", ",");
        }
        if (scale < 0)
        {
            return Convert.ToDecimal(str);
        }
        return Math.Round(Convert.ToDecimal(str), scale);
    }
    private static string getIntValue(string str)
    {
        if (str.Length == 0)
        {
            return "0";
        }
        str = str.Replace(" ", "");
        if (str.IndexOf('.') >= 0)
        {
            str = str.Substring(0, str.IndexOf('.'));
            //return Convert.ToInt32(str.ToDecimal());
        }
        return str;
    }
    public static Int16 ToInt16(this string str)
    {
        return Convert.ToInt16(getIntValue(str));
    }
    public static Int32 ToInt32(this string str, int type = 32)
    {
        return Convert.ToInt32(getIntValue(str));
    }
    public static Int64 ToLong(this string str)
    {
        return Convert.ToInt64(getIntValue(str));
    }
    public static Int64 ToInt64(this string str)
    {
        return Convert.ToInt64(getIntValue(str));
    }

    /// <summary>
    /// Конвертирует строки формата YYYYMMDD или YYMMDD в формат даты
    /// </summary>
    /// <param name="str"></param>
    /// <param name="yearNumeral">Кол-во цифр в годе (default = 4)</param>
    /// <returns>DateTime</returns>
    public static DateTime ConvertToDateForm_YYYYMMDD(this string str, int yearNumeral = 4)
    {
        if (str.Empty()) return DateTime.MinValue;
        var date = new DateMarket();
        date.SetDay(str.Substring(6, 2)).SetMonth(str.Substring(4, 2)).SetYear(str.Substring(0, 4));
        return date.GetDateTime();
    }
}
