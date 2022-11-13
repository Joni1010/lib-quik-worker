using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Components.Log
{
    public class QLog
    {
        public const string DEFAULT_LOG_FILE = @"info.log";
        /// <summary>
        /// Путь к лог файлу
        /// </summary>
        public static string FilenameLog = DEFAULT_LOG_FILE;
        /// <summary>
        /// 
        /// </summary>
        public static Func<Exception, object> ActionError = null;
        /// <summary>
        /// Записать текст в файл лога
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Write(string text)
        {
            try
            {
                text = DateTime.Now.ToString() + ": " + text + "\n";
                System.IO.StreamWriter textFile = new System.IO.StreamWriter(FilenameLog, true);
                textFile.WriteLine(text);
                textFile.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Функция отлова исключений и записи их в лог
        /// </summary>
        /// <param name="action">Обрабатываемое действие</param>
        public static void CatchException(Action action)
        {
            QLog.CatchException(action, "");
        }
        /// <summary>
        /// Функция отлова исключений и записи их в лог
        /// </summary>
        /// <param name="action">Обрабатываемое действие</param>
        /// <param name="appendText">Add text message.</param>
        public static void CatchException(Action action, string appendText = "", Action catchAction = null)
        {
            try
            {
                if (!action.Empty())
                    action();
            }
            catch (Exception e)
            {
                QLog.Write(e.ToString() + appendText);
                if (QLog.ActionError.NotIsNull())
                {
                    QLog.ActionError(e);
                }
            }
        }

        /// <summary>
        /// Функция отлова исключений и записи их в лог
        /// </summary>
        /// <param name="action">Обрабатываемое действие</param>
        public static object CatchException(Func<object> action)
        {
            return QLog.CatchException(action, "");
        }
        public static object CatchException(Func<object> action, string appendText = "")
        {
            try
            {
                if (!action.Empty())
                    return action();
            }
            catch (Exception e)
            {
                QLog.Write(e.ToString() + appendText);
                if (QLog.ActionError.NotIsNull())
                {
                    QLog.ActionError(e);
                }
            }
            return null;
        }
    }
}
