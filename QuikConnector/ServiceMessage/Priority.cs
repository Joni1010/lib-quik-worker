using QuikConnector.ServiceMessage;
using QuikConnector.ServiceMessage.Message;

namespace ServiceMessage
{
    class Priority
    {
        public const int P_OTHER = 1000;
        //base
        public const int P_FIRM = 1;
        public const int P_CLIENT = 2;
        public const int P_CLASS = 3;
        public const int P_ACCOUNT = 4;
        public const int P_SECURITIES = 5;
        public const int P_CHECKSECURITIES = 6;
        public const int P_GIVETRADES = 7;

        //major
        public const int P_ORDER = 10;
        public const int P_STOPORDER = 10;

        public const int P_MONEYLIMIT = 20;
        public const int P_DEPOLIMIT = 20;
        public const int P_FUTURESHOLDING = 20;
        public const int P_FUTURESLIMIT = 20;
        public const int P_FUTURESLIMITCHANGE = 20;
        public const int P_MYTRADE = 20;

        public const int P_TRADE = 30;
        public const int P_PORTFOLIOINFO = 40;
        public const int P_REPLY = 50;
        public const int P_CONNECTION = 60;

        public const int P_HISTORYTRADE = 100;
        public const int P_CHANGESECURITIES = 200;
        public const int P_QUOTE = 300;

        public static bool NotPriority(Msg msg)
        {
            return
                msg.Code() == MsgCodes.CODE_MSG_TYPE_TRADE_HISTORY ||
                msg.Code() == MsgCodes.CODE_MSG_TYPE_CHECKSECURITIES || 
                msg.Code() == MsgCodes.CODE_MSG_TYPE_SERVICE
                ;
        }

    }
}
