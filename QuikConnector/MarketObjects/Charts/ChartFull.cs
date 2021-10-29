using System;

namespace MarketObjects.Charts
{
    /// <summary> Котировка цены и объема buy/sell</summary>
    [Serializable]
    public class ChartFull
    {
        /// <summary> Цена </summary>
        public decimal Price = -100000;
        /// <summary> Объем buy </summary>
        public long VolBuy = 0;
        /// <summary> Объем sell </summary>
        public long VolSell = 0;
        /// <summary> Кол-во сделок покупки </summary>
        public long CountBuy = 0;
        /// <summary> Кол-во сделок продажи </summary>
        public long CountSell = 0;
    }
}
