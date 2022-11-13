using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SOrder
    {
        [DataMember]
        internal string side_qualifier = "";
        [DataMember]
        internal string settlecode = "";
        [DataMember]
        internal string repo2value = "";
        [DataMember]
        internal string canceled_uid = "";
        [DataMember]
        internal SDate datetime = new SDate();
        [DataMember]
        internal string value = "";
        [DataMember]
        internal string qty = "";
        [DataMember]
        internal string repo_value_balance = "";
        [DataMember]
        internal string accruedint = "";
        [DataMember]
        internal string expiry_time = "";
        [DataMember]
        internal string reject_reason = "";
        [DataMember]
        internal string repovalue = "";
        [DataMember]
        internal string userid = "";
        [DataMember]
        internal string executing_trader_qualifier = "";
        [DataMember]
        internal string investment_decision_maker_short_code = "";
        [DataMember]
        internal string price = "";
        [DataMember]
        internal string brokerref = "";
        [DataMember]
        internal string price2 = "";
        [DataMember]
        internal string acnt_type = "";
        [DataMember]
        internal string client_short_code = "";
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string exchange_code = "";
        [DataMember]
        internal string ordernum = "";
        [DataMember]
        internal string bank_acc_id = "";
        [DataMember]
        internal string client_qualifier = "";
        [DataMember]
        internal string on_behalf_of_uid = "";
        [DataMember]
        internal string uid = "";
        [DataMember]
        internal string trans_id = "";
        [DataMember]
        internal string class_code = "";
        [DataMember]
        internal string extref = "";
        [DataMember]
        internal string value_entry_type = "";
        [DataMember]
        internal string exec_type = "";
        [DataMember]
        internal string ext_order_flags = "";
        [DataMember]
        internal string settle_currency = "";
        [DataMember]
        internal string balance = "";
        [DataMember]
        internal string client_code = "";
        [DataMember]
        internal string price_currency = "";
        [DataMember]
        internal string linkedorder = "";
        [DataMember]
        internal string order_num = "";
        [DataMember]
        internal string account = "";
        [DataMember]
        internal string revision_number = "";
        [DataMember]
        internal string executing_trader_short_code = "";
        [DataMember]
        internal string awg_price = "";
        [DataMember]
        internal string yield = "";
        [DataMember]
        internal string visible = "";
        [DataMember]
        internal string expiry = "";
        [DataMember]
        internal string filled_value = "";
        [DataMember]
        internal string sec_code = "";
        [DataMember]
        internal string capacity = "";
        [DataMember]
        internal string investment_decision_maker_qualifier = "";
        [DataMember]
        internal string min_qty = "";
        [DataMember]
        internal SDate withdraw_datetime = new SDate();
        [DataMember]
        internal string start_discount = "";
        [DataMember]
        internal string repoterm = "";
        [DataMember]
        internal string passive_only_order = "";
        [DataMember]
        internal string seccode = "";
        [DataMember]
        internal string activation_time = "";
        [DataMember]
        internal string accepted_uid = "";
        [DataMember]
        internal string flags = "";
        [DataMember]
        internal string ext_order_status = "";
    }
}
