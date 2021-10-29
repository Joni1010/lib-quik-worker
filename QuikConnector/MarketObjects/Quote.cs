using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MarketObjects
{

    
    /// <summary> Стакан </summary>
    [Serializable]
    public class Quote
    {
        /// <summary> Строка в стакане </summary>
        public class QuoteRow
        {
            /// <summary> Цена </summary>
            public decimal Price;
            /// <summary> Объем </summary>
            public int Volume;
        };
        /// <summary> Инструмент стакана </summary>
        public Securities Sec;
        /// <summary> Массив цен на покупку </summary>
        public QuoteRow[] Bid;
        /// <summary> Массив цен на продажу </summary>
        public QuoteRow[] Ask;          
    }
}
