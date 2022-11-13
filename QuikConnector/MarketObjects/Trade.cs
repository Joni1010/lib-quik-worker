using System;
using QuikConnector.Components.Log;
using QuikConnector.MarketObjects;


namespace MarketObjects
{
    [Serializable]
    public class MyTrade
    {
        /// <summary> Номер заявки по которой прошла сделка </summary>
        public long OrderNum;
        /// <summary> Заявка по которой совершена сделка </summary>
        public Order Order;
        /// <summary> Сделка (параметры) </summary>
        public Trade Trade;

        public long uid;
        /// <summary> Комиссия брокера </summary>
        public decimal BrokerComission;
        /// <summary> Клиринговая комиссия </summary>
        public decimal ClearingComission;
        /// <summary> Комиссия Фондовой биржи </summary>
        public decimal ExchangeComission;
        /// <summary> Блокировка обеспечения </summary>
        public decimal BlockSecurities;
        /// <summary> Коментарий </summary>
        public string Comment;

        public MyTrade() { }
        public MyTrade(Trade trade)
        {
            this.Trade = trade;
        }
    }
    [Serializable]
    public class Trade
    {
        /// <summary> Номер сделки </summary>
        public long Number;
        /// <summary> Инструмент </summary>
        public Securities Sec;
        /// <summary> Код инструмента </summary>
        public string SecCode;
        /// <summary> Цена сделки </summary>
        public decimal Price;
        /// <summary> Объем сделки </summary>
        public int Volume;
        /// <summary> Направление сделки </summary>
        public OrderDirection? Direction { set; get; }
        /// <summary> Время сделки </summary>
        public DateMarket DateTrade;
        /// <summary> Открытый интерес </summary>
        public decimal OpenInterest;


		/// <summary> 
		/// Конвертирует строку в сделку.
		/// Формат: DATE;TIME;LAST;VOL;ID;OPER
		/// </summary>
		/// <param name="sec">Инструмент</param>
		/// <param name="strTrade">Строка с данными по сделке </param>
		/// <returns></returns>
		public static Trade GetConvertTrade(Securities sec, string strTrade)
		{
			if (sec.IsNull()) return null;
			if (strTrade.Length == 0) return null;
            return (Trade)QLog.CatchException(() =>
            {
                var data = strTrade.Split(';');
                if (data.Length < 4)
                {
                    data = strTrade.Split(',');
                    if (data.Length < 4) return null;
                }
                DateMarket date = new DateMarket();
                date.SetDay(data[0].Substring(6, 2)).SetMonth(data[0].Substring(4, 2)).SetYear(data[0].Substring(0, 4));
                date.SetHour(data[1].Substring(0, 2)).SetMinute(data[1].Substring(2, 2)).SetSecond(data[1].Substring(4, 2));

                return new Trade()
                {
                    DateTrade = date,
                    Price = Math.Round(data[2].ToDecimal(), sec.Scale),
                    Volume = data[3].ToInt32(),
                    Number = data[4].ToLong(),
                    Direction = data[5] == "B" ? OrderDirection.Buy : OrderDirection.Sell,
                    Sec = sec,
                    SecCode = sec.Code
                };
            });
		}

        /// <summary> Если sell то true, иначе false </summary>
        /// <returns></returns>
        public bool IsSell()
        {
            return Direction == OrderDirection.Sell ? true : false;
        }
        /// <summary> Если buy то true, иначе false </summary>
        /// <returns></returns>
        public bool IsBuy()
        {
            return Direction == OrderDirection.Buy ? true : false;
        }
    };
}
