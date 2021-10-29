using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.MarketObjects.Structures
{
    [DataContract]
    class SAccount
    {
        /// <summary>
        /// 	Список кодов классов, разделенных символом «|»
        /// </summary>
        [DataMember]
        internal string class_codes = "";
        /// <summary>
        /// 	Идентификатор фирмы
        /// </summary>
        [DataMember]
        internal string firmid = "";
        /// <summary>
        /// Код торгового счета
        /// </summary>        
        [DataMember]
        internal string trdaccid = "";
        /// <summary>
        /// Описание
        /// </summary>        
        [DataMember]
        internal string description = "";
        /// <summary>
        /// Запрет необеспеченных продаж. Возможные значения: «0» – Нет; «1» – Да;
        /// </summary>
        [DataMember]
        internal string fullcoveredsell = "";
        /// <summary>
        /// 	Номер основного торгового счета
        /// </summary>
        [DataMember]
        internal string main_trdaccid = ""; 
        /// <summary>
        /// 	Расчетная организация по «Т0»
        /// </summary>
        [DataMember]
        internal string bankid_t0 = ""; 
        /// <summary>
        /// 	Расчетная организация по «Т+»
        /// </summary>
        [DataMember]
        internal string bankid_tplus = ""; 
        /// <summary>
        /// 	Тип торгового счета
        /// </summary>
        [DataMember]
        internal string trdacc_type = ""; 
        /// <summary>
        /// 	Раздел счета Депо
        /// </summary>
        [DataMember]
        internal string depunitid = ""; 
        /// <summary>
        /// 	Статус торгового счета. Возможные значения: «0» – операции разрешены; «1» – операции запрещены
        /// </summary>
        [DataMember]
        internal string status = ""; 
        /// <summary>
        /// 	Тип раздела. Возможные значения: «0» – раздел обеспечения; иначе – для торговых разделов
        /// </summary>
        [DataMember]
        internal string firmuse = ""; 
        /// <summary>
        /// 	Номер счета депо в депозитарии
        /// </summary>
        [DataMember]
        internal string depaccid = ""; 
        /// <summary>
        /// 	Код дополнительной позиции по денежным средствам
        /// </summary>
        [DataMember]
        internal string bank_acc_id = ""; 
    }
}
