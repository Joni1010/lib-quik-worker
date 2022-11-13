using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SMyTrade
    {
        [DataMember]
        internal string liquidity_indicator = "";
        [DataMember]
        internal string repo2value = "";
        [DataMember]
        internal string clearing_firmid = "";
        [DataMember]
        internal string canceled_uid = "";
        [DataMember]
        internal string clearing_comission = "";
        [DataMember]
        internal string broker_comission = "";
        [DataMember]
        internal string kind = "";
        [DataMember]
        internal string order_exchange_code = "";
        [DataMember]
        internal string upper_discount = "";
        [DataMember]
        internal string trade_num = "";
        [DataMember]
        internal string price2 = "";
        [DataMember]
        internal string order_price = "";
        [DataMember]
        internal string exchange_code = "";
        [DataMember]
        internal string seccode = "";
        [DataMember]
        internal SDate canceled_datetime = new SDate();
        [DataMember]
        internal string cross_rate = "";
        [DataMember]
        internal string station_id = "";
        [DataMember]
        internal string extref = "";
        [DataMember]
        internal string start_discount = "";
        [DataMember]
        internal string repoterm = "";
        [DataMember]
        internal string ext_trade_flags = "";
        [DataMember]
        internal string yield = "";
        [DataMember]
        internal string settle_date = "";
        [DataMember]
        internal string order_qty = "";
        [DataMember]
        internal string cpfirmid = "";
        [DataMember]
        internal string sec_code = "";
        [DataMember]
        internal string on_behalf_of_uid = "";
        [DataMember]
        internal string side_qualifier = "";
        [DataMember]
        internal string settlecode = "";
        [DataMember]
        internal SDate datetime = new SDate();
        [DataMember]
        internal string exec_market = "";
        [DataMember]
        internal string value = "";
        [DataMember]
        internal string client_code = "";
        [DataMember]
        internal string accruedint = "";
        [DataMember]
        internal string otc_post_trade_indicator = "";
        [DataMember]
        internal string repovalue = "";
        [DataMember]
        internal string client_short_code = "";
        [DataMember]
        internal string accrued2 = "";
        [DataMember]
        internal string executing_trader_qualifier = "";
        [DataMember]
        internal string brokerref = "";
        [DataMember]
        internal string block_securities = "";
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string bank_acc_id = "";
        [DataMember]
        internal string tradenum = "";
        [DataMember]
        internal string trans_id = "";
        [DataMember]
        internal string class_code = "";
        [DataMember]
        internal string tech_center_comission = "";
        [DataMember]
        internal string settle_currency = "";
        [DataMember]
        internal string capacity = "";
        [DataMember]
        internal string exchange_comission = "";
        [DataMember]
        internal string mleg_base_sid = "";
        [DataMember]
        internal string userid = "";
        [DataMember]
        internal string account = "";
        [DataMember]
        internal string period = "";
        [DataMember]
        internal string executing_trader_short_code = "";
        [DataMember]
        internal string qty = "";
        [DataMember]
        internal string client_qualifier = "";
        [DataMember]
        internal string price = "";
        [DataMember]
        internal string clearing_bank_accid = "";
        [DataMember]
        internal string waiver_flag = "";
        [DataMember]
        internal string lseccode = "";
        [DataMember]
        internal string ordernum = "";
        [DataMember]
        internal string investment_decision_maker_qualifier = "";
        [DataMember]
        internal string linked_trade = "";
        [DataMember]
        internal string reporate = "";
        [DataMember]
        internal string lower_discount = "";
        [DataMember]
        internal string order_revision_number = "";
        [DataMember]
        internal string trade_currency = "";
        [DataMember]
        internal string system_ref = "";
        [DataMember]
        internal string order_num = "";
        [DataMember]
        internal string uid = "";
        [DataMember]
        internal string flags = "";
        [DataMember]
        internal string investment_decision_maker_short_code = "";
    }
}
