using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SFuturesHolding
    {
        [DataMember]
        internal string startnet = "";
        [DataMember]
        internal string opensells = "";
        [DataMember]
        internal string trdaccid = "";
        [DataMember]
        internal string openbuys = "";
        [DataMember]
        internal string totalnet = "";
        [DataMember]
        internal string real_varmargin = "";
        [DataMember]
        internal string varmargin = "";
        [DataMember]
        internal string todaybuy = "";
        [DataMember]
        internal string startbuy = "";
        [DataMember]
        internal string total_varmargin = "";
        [DataMember]
        internal string seccode = "";
        [DataMember]
        internal string positionvalue = "";
        [DataMember]
        internal string avrposnprice = "";
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string cbplplanned = "";
        [DataMember]
        internal string session_status = "";
        [DataMember]
        internal string sec_code = "";
        [DataMember]
        internal string cbplused = "";
        [DataMember]
        internal string todaysell = "";
        [DataMember]
        internal string type = "";
        [DataMember]
        internal string startsell = "";

    }
}
