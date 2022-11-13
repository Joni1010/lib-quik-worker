using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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