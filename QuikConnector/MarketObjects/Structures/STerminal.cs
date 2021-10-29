using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    internal class STerminal
    {
        [DataMember]
        internal string VERSION = "";
        [DataMember]
        internal string TRADEDATE = "";
        [DataMember]
        internal string SERVERTIME = "";
        [DataMember]
        internal string LASTRECORDTIME = "";
        [DataMember]
        internal string NUMRECORDS = "";
        [DataMember]
        internal string LASTRECORD = "";
        [DataMember]
        internal string LATERECORD = "";
        [DataMember]
        internal string CONNECTION = "";
        [DataMember]
        internal string IPADDRESS = "";
        [DataMember]
        internal string IPPORT = "";
        [DataMember]
        internal string IPCOMMENT = "";
        [DataMember]
        internal string SERVER = "";
        [DataMember]
        internal string SESSIONID = "";
        [DataMember]
        internal string USER = "";
        [DataMember]
        internal string USERID = "";
        [DataMember]
        internal string ORG = "";
        [DataMember]
        internal string MEMORY = "";
        [DataMember]
        internal string LOCALTIME = "";
        [DataMember]
        internal string CONNECTIONTIME = "";
        [DataMember]
        internal string MESSAGESSENT = "";
        [DataMember]
        internal string ALLSENT = "";
        [DataMember]
        internal string BYTESSENT = "";
        [DataMember]
        internal string BYTESPERSECSENT = "";
        [DataMember]
        internal string MESSAGESRECV = "";
        [DataMember]
        internal string BYTESRECV = "";
        [DataMember]
        internal string ALLRECV = "";
        [DataMember]
        internal string BYTESPERSECRECV = "";
        [DataMember]
        internal string AVGSENT = "";
        [DataMember]
        internal string AVGRECV = "";
        [DataMember]
        internal string LASTPINGTIME = "";
        [DataMember]
        internal string LASTPINGDURATION = "";
        [DataMember]
        internal string AVGPINGDURATION = "";
        [DataMember]
        internal string MAXPINGTIME = "";
        [DataMember]
        internal string MAXPINGDURATION = "";
    }
}
