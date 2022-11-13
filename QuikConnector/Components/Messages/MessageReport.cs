namespace QuikConnector.Components.Messages
{
    public struct MessageReport
    {
        /// <summary>
        /// 
        /// </summary>
        public string Reply;
        /// <summary>
        /// 
        /// </summary>
        public string[] Response;
        /// <summary>
        /// 
        /// </summary>
        public object Object;
        /// <summary>
        /// Формируем обьект ответа обработки сообщения
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reply"></param>
        /// <returns></returns>
        public static MessageReport Create(object obj, string reply = "")
        {
            return new MessageReport() { Object = obj, Reply = reply };
        }

    }
}
