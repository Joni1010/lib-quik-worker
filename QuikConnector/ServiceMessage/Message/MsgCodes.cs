using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuikConnector.ServiceMessage
{
    class MsgCodes
    {
        public const string CODE_MSG_TYPE_SERVICE       = "01";
        public const string CODE_MSG_TYPE_START_TRADES  = "02";

        public const string CODE_MSG_TYPE_FIRM = "10";
        public const string CODE_MSG_TYPE_CLASS = "11";
        public const string CODE_MSG_TYPE_ACCOUNT = "12";
        public const string CODE_MSG_TYPE_CLIENT = "13";

        public const string CODE_MSG_TYPE_SECURITIES = "20";
        public const string CODE_MSG_TYPE_CHECKSECURITIES = "21";

        public const string CODE_MSG_TYPE_DEPOLIMIT		    = "30";
        public const string CODE_MSG_TYPE_MONEYLIMIT        = "31";
        public const string CODE_MSG_TYPE_FUTURESHOLDING    = "32";
        public const string CODE_MSG_TYPE_FUTURESLIMIT      = "33";


        public const string CODE_MSG_TYPE_ORDER             = "40";
        public const string CODE_MSG_TYPE_STOPORDER         = "41";
        public const string CODE_MSG_TYPE_MYTRADE           = "42";
        public const string CODE_MSG_TYPE_TRADE             = "43";
        public const string CODE_MSG_TYPE_TRADE_HISTORY     = "44";
    }
}
