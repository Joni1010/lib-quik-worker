using System;

namespace MarketObjects
{
    /// <summary> Рыночные классы </summary>
	[Serializable]
    public class MarketClass
    {
        /// <summary> Класс фирмы </summary>
        public Firm Firm = null;
        /// <summary> ID фирмы </summary>
        public string FirmId;
        /// <summary> Название Класса </summary>
        public string Name = null;
        /// <summary> Код класса </summary>
        public string Code = null;
        /// <summary> Кол-во параметров </summary>
        public int CountParams = 0;
        /// <summary> Кол-во инструментов в классе </summary>
        public int CountSecurities = 0;
    }
}
