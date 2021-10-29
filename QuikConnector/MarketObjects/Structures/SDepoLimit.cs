using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SDepoLimit
    {
        [DataMember]
        internal string limit_kind = "";
        [DataMember]
        internal string locked_sell = "";
        [DataMember]
        internal string client_code = "";
        [DataMember]
        internal string awg_position_price = "";
        [DataMember]
        internal string locked_buy = "";
        [DataMember]
        internal string openlimit = "";
        [DataMember]
        internal string sec_code = "";
        [DataMember]
        internal string wa_position_price = "";
        [DataMember]
        internal string openbal = "";
        [DataMember]
        internal string currentbal = "";
        [DataMember]
        internal string trdaccid = "";
        [DataMember]
        internal string locked_sell_value = "";
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string currentlimit = "";
        [DataMember]
        internal string locked_buy_value = "";
    }
}
