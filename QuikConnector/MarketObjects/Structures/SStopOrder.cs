using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SStopOrder
    {
        [DataMember]
        internal string uid = "";
        [DataMember]
        internal string canceled_uid = "";
        [DataMember]
        internal string stopflags = "";
        [DataMember]
        internal string condition = "";
        [DataMember]
        internal string co_order_num = "";
        [DataMember]
        internal string qty = "";
        [DataMember]
        internal string price = "";
        [DataMember]
        internal string brokerref = "";
        [DataMember]
        internal string client_code = "";
        [DataMember]
        internal string condition_sec_code = "";
        [DataMember]
        internal SDate order_date_time;
        [DataMember]
        internal string trans_id = "";
        [DataMember]
        internal string alltrade_num = "";
        [DataMember]
        internal string class_code = "";
        [DataMember]
        internal SDate activation_date_time;
        [DataMember]
        internal string account = "";
        [DataMember]
        internal string condition_class_code = "";
        [DataMember]
        internal string balance = "";
        [DataMember]
        internal string withdraw_time = "";
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string seccode = "";
        [DataMember]
        internal string stop_order_type = "";
        [DataMember]
        internal SDate withdraw_datetime;
        [DataMember]
        internal string ordertime = "";
        [DataMember]
        internal string flags = "";
        [DataMember]
        internal string condition_seccode = "";
        [DataMember]
        internal string sec_code = "";
        [DataMember]
        internal string active_to_time = "";
        [DataMember]
        internal string active_from_time = "";
        [DataMember]
        internal string condition_price = "";
        [DataMember]
        internal string filled_qty = "";
        [DataMember]
        internal string orderdate = "";
        [DataMember]
        internal string ordernum = "";
        [DataMember]
        internal string co_order_price = "";
        [DataMember]
        internal string condition_price2 = "";
        [DataMember]
        internal string linkedorder = "";
        [DataMember]
        internal string spread = "";
        [DataMember]
        internal string offset = "";
        [DataMember]
        internal string expiry = "";
        [DataMember]
        internal string order_num = "";
    }
}
