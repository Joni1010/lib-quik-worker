using QuikConnector.ServiceMessage.Message;
using System;

namespace QuikConnector.ServiceMessage.Package
{
    /// <summary>
    ///  Структура входящего сообщения, с указанием приоритета. Разбитая на массив.
    /// </summary>
    public class PackMsg
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static PackMsg Parse(string pack)
        {
            var newPack = new PackMsg();
            newPack.Data = pack.Split(SPLITTER_PACK);
            return newPack;
        }
        /// <summary> 
        /// Разделитель сообщений в одной посылке 
        /// </summary>
        protected const char SPLITTER_PACK = '\t';
        /// <summary>
        /// Структура пакета
        /// </summary>
        protected string[] Data;  
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetData()
        {
            return Data;
        }
    }
}
