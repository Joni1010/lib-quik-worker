using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SClass
    {
        /// <summary>
        /// Идентификатор фирмы
        /// </summary>
        [DataMember]
        internal string firmid = "";
        /// <summary>
        /// Название класса
        /// </summary>
        [DataMember]
        internal string name = "";
        /// <summary>
        /// Код класса
        /// </summary>
        [DataMember]
        internal string code = "";
        /// <summary>
        /// Количество параметров в классе
        /// </summary>
        [DataMember]
        internal string npars = "";
        /// <summary>
        /// Количество бумаг в классе
        /// </summary>
        [DataMember]
        internal string nsecs = "";
    }
}
