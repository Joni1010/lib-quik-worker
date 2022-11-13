namespace QuikConnector.Components.Messages
{
    /// <summary>
    ///  Структура входящего сообщения, с указанием приоритета. Разбитая на массив.
    /// </summary>
    public class MessagePackage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static MessagePackage Parse(string pack)
        {
            var newPack = new MessagePackage();
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
