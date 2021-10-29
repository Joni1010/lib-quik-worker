using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.Logs
{
    public class Qlog
    {
        /// <summary>
        /// Записать текст в файл лога
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static public bool Write(string text)
        {
            try
            {
                text = DateTime.Now.ToString() + ": " + text + "\n";
                System.IO.StreamWriter textFile = new System.IO.StreamWriter(@"info.log", true);
                textFile.WriteLine(text);
                textFile.Close();
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// Функция отлова исключений и записи их в лог
        /// </summary>
        /// <param name="action">Обрабатываемое действие</param>
        static public void CatchException(Action action)
        {
			Qlog.CatchException(action, "");
		}
		/// <summary>
		/// Функция отлова исключений и записи их в лог
		/// </summary>
		/// <param name="action">Обрабатываемое действие</param>
		/// <param name="appendText">Add text message.</param>
		static public void CatchException(Action action, string appendText = "", Action catchAction = null)
		{
			try
			{
				if (!action.Empty())
					action();
			}
			catch (Exception e)
			{
				Qlog.Write(e.ToString() + appendText);
				if (catchAction.NotIsNull()) catchAction();
			}
		}

		/// <summary>
		/// Функция отлова исключений и записи их в лог
		/// </summary>
		/// <param name="action">Обрабатываемое действие</param>
		static public object CatchException(Func<object> action)
		{
			return Qlog.CatchException(action, "");
		}
		static public object CatchException(Func<object> action, string appendText = "", Action catchAction = null)
		{
			try
			{
				if (!action.Empty())
					return action();
			}
			catch (Exception e)
			{
				Qlog.Write(e.ToString() + appendText);
				if (catchAction.NotIsNull()) catchAction();
			}
			return null;
		}
	}
}
