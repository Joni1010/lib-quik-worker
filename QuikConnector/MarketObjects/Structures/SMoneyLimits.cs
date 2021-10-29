using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SMoneyLimits
    {
        [DataMember]
        internal string currcode = "";
        [DataMember]
        internal string locked = "";
        [DataMember]
        internal string positions_collateral = "";
        [DataMember]
        internal string client_code = "";
        [DataMember]
        internal string openbal = "";
        [DataMember]
        internal string leverage = "";
        [DataMember]
        internal string openlimit = "";
        [DataMember]
        internal string wa_position_price = "";
        [DataMember]
        internal string tag = "";
        [DataMember]
        internal string locked_margin_value = "";
        [DataMember]
        internal string currentbal = "";
        [DataMember]
        internal string limit_kind = "";
        [DataMember]
        internal string locked_value_coef = "";
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string currentlimit = "";
        [DataMember]
        internal string orders_collateral = "";
    }
}
