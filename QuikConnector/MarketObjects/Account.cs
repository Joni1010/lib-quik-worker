using System.Collections.Generic;

namespace MarketObjects
{
    /// <summary> Класс счета</summary>
    public class Account
    {
        /// <summary>
        /// Список классов принадлежащих данному счету
        /// </summary>
        public List<MarketClass> AccClasses = new List<MarketClass>(); 
        /// <summary>
        /// Класс фирмы
        /// </summary>
        public Firm Firm;
        /// <summary>
        /// ID  счета
        /// </summary>
        public string AccID;
        /// <summary>
        /// Тип счета
        /// </summary>
        public int AccType;
    }
}
