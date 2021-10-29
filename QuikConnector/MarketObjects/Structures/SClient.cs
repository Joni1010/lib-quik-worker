using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SClient
    {
        /// <summary>
        /// Код клиента
        /// </summary>
        [DataMember]
        internal string client_codes = "";

    }
}
