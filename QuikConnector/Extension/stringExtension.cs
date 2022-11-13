using QuikConnector.MarketObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
