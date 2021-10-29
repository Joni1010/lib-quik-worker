using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SFuturesLimit
    {
        [DataMember]
        internal string trdaccid = "";
        [DataMember]
        internal string accruedint = "";
        [DataMember]
        internal string limit_type = "";
        [DataMember]
        internal string currcode = "";
        [DataMember]
        internal string real_varmargin = "";
        [DataMember]
        internal string cbplused_for_positions = "";
        [DataMember]
        internal string ts_comission = "";
        [DataMember]
        internal string kgo = "";
        [DataMember]
        internal string options_premium = "";
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string cbplplanned = "";
        [DataMember]
        internal string cbplused_for_orders = "";
        [DataMember]
        internal string varmargin = "";
        [DataMember]
        internal string liquidity_coef = "";
        [DataMember]
        internal string cbplused = "";
        [DataMember]
        internal string cbp_prev_limit = "";
        [DataMember]
        internal string cbplimit = "";
    }
}
