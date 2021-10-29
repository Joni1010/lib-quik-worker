using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class STrade
    {
        [DataMember]
        internal string class_code = "";
        [DataMember]
        internal string value = "";
        [DataMember]
        internal string yield = "";
        [DataMember]
        internal string trade_num = "";
        [DataMember]
        internal string exec_market = "";
        [DataMember]
        internal string period = "";
        [DataMember]
        internal string tradenum = "";
        [DataMember]
        internal string price = "";
        [DataMember]
        internal SDate datetime;
        [DataMember]
        internal string repoterm = "";
        [DataMember]
        internal string sec_code = "";
        [DataMember]
        internal string repo2value = "";
        [DataMember]
        internal string exchange_code = "";
        [DataMember]
        internal string seccode = "";
        [DataMember]
        internal string open_interest = "";
        [DataMember]
        internal string qty = "";
        [DataMember]
        internal string accruedint = "";
        [DataMember]
        internal string repovalue = "";
        [DataMember]
        internal string flags = "";
        [DataMember]
        internal string reporate = "";
        [DataMember]
        internal string settlecode = "";
    }
}
