using QuikConnector.ServiceMessage.Message;
using System;

namespace QuikConnector.ServiceMessage.Package
{
    /// <summary>
    ///  Структура входящего сообщения, с указанием приоритета. Разбитая на массив.
    /// </summary>
    public class PackMsg
    {
        public delegate void ActionMsg(Msg msg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static PackMsg Create(string pack)
        {
            return new PackMsg() { Data = pack.Split(SPLITTER_PACK) };
        }
        /// <summary> 
        /// Разделитель сообщений в одной посылке 
        /// </summary>
        protected const char SPLITTER_PACK = '\t';

        /// <summary>
        /// Приоритет пакета
        /// </summary>
        public int priority;
        /// <summary>
        /// Структура пакета
        /// </summary>
        public string[] Data;
        /// <summary>
        /// Флаг на проверку дублей
        /// </summary>
        public bool DelDouble;
        /// <summary>
        /// Условие удаления дубликатов
        /// </summary>
        public Func<PackMsg, bool> ConditionDelDouble;
        /// <summary>
        /// Кол-во попыток обработки сообщения
        /// </summary>
        public int CountTry;

        public void Each(ActionMsg actionForMsg)
        {
            if (actionForMsg is null)
            {
                return;
            }
            foreach (var item in Data)
            {
                if (item.Length > 0)
                {
                    actionForMsg(Msg.Create(item));
                }
            }
        }
    }
}
