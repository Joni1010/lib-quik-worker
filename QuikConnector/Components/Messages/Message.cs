namespace QuikConnector.Components.Messages
{
    public class Message
    {
        /// <summary>
        /// 
        /// </summary>
        private string content = "";
        /// <summary>
        /// 
        /// </summary>
        private string code = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Message Create(string data)
        {
            var msg = new Message() { content = data };
            msg.init();
            return msg;
        }
        private void init()
        {
            code = content.Substring(0, 2);
            content = content.Remove(0, 2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Content()
        {
            return content;
        }
        public string Code()
        {
            return code;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return content; 
        }
    }
}
