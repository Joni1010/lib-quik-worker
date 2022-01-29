using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.ServiceMessage.Message
{
    public struct MsgReport
    {
        /// <summary>
        /// 
        /// </summary>
        public string Reply;
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
        public static MsgReport Create(object obj, string reply = "")
        {
            return new MsgReport() { Object = obj, Reply = reply };
        }

    }
}
