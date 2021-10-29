using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    internal class SFirm
    {
        [DataMember]
        internal string firmid = "";
        [DataMember]
        internal string firm_name = "";
        [DataMember]
        internal string status = "";
        [DataMember]
        internal string exchange = "";
    }
}
