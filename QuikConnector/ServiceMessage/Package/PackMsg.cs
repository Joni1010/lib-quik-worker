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
            return new PackMsg() { Data = ConvertToArrayMsg(pack.Split(SPLITTER_PACK)) };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected static Msg[] ConvertToArrayMsg(string[] data)
        {
            Msg[] collection = new Msg[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Length > 0)
                {
                    collection[i] = Msg.Create(data[i]);
                }
            }
            return collection;
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
        public Msg[] Data;
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
                actionForMsg(item);
            }
        }
    }
}
