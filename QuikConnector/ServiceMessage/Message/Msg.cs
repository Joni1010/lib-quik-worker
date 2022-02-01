using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.ServiceMessage.Message
{
    public class Msg
    {
        private string content = "";
        private string code = "";
        /// <summary>
        /// Разделитель данных в пакете
        /// </summary>
        public const char SP_DATA = '#';
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Msg Create(string data)
        {
            var msg = new Msg() { content = data };
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
