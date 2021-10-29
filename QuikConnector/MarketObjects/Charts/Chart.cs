
using System;

namespace MarketObjects.Charts
{
    /// <summary> Котировка цены и объема</summary>
    [Serializable]
    public class Chart
    {
        /// <summary> Цена </summary>
        public decimal Price = 0;
        /// <summary> Объем </summary>
        public long Volume = 0;
    }
}
