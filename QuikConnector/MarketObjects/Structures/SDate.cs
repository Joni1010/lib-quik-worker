using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SDate
    {
        [DataMember]
        internal string ms = "";
        [DataMember]
        internal string sec = "";
        [DataMember]
        internal string hour = "";
        [DataMember]
        internal string mcs = "";
        [DataMember]
        internal string year = "";
        [DataMember]
        internal string day = "";
        [DataMember]
        internal string min = "";
        [DataMember]
        internal string week_day = "";
        [DataMember]
        internal string month = "";

        public DateMarket GetDate()
        {
            DateMarket date = new DateMarket();
            date.SetYear(year);
            date.SetMonth(month);
            date.SetDay(day);
            date.SetHour(hour);
            date.SetMinute(min);
            date.SetSecond(sec);
            date.SetMiliSecond(ms);
            return date;
        }
    }
}
