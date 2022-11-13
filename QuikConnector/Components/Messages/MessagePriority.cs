using QuikConnector.Components;
using QuikConnector.Components.Messages;

namespace QuikConnector.Components.Messages
{
    class MessagePriority
    {
        public static bool NotPriority(Message msg)
        {
            return
                msg.Code() == MessageCode.CODE_MSG_TYPE_TRADE_HISTORY ||
                msg.Code() == MessageCode.CODE_MSG_TYPE_CHECKSECURITIES || 
                msg.Code() == MessageCode.CODE_MSG_TYPE_SERVICE
                ;
        }

    }
}
