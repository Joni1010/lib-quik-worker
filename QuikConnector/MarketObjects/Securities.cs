using QuikConnector.MarketObjects;
using System;

namespace MarketObjects
{
    /// <summary>
    /// Класс торговых инструментов
    /// </summary>
    [Serializable]
    public class Securities
    {
        /// <summary>
        /// Статус инструмента 0 - не торгуется, 1 - торгуется, 2 - приостановлен
        /// </summary>
        public int Status
        {
            get { return this.Params.Status; }
            set { this.Params.Status = value; }
        }
        /// <summary> Code </summary>
        public string Code = null;
        /// <summary> Наименование инструмента  (STRING)   </summary>
        public string Name = null;
        /// <summary> Короткое наименование инструмента(STRING) </summary>
        public string Shortname = "";
        /// <summary> Класс инструмента </summary>
        public MarketClass Class = null;
        /// <summary> Код класса инструмента </summary>
        public string ClassCode
        {
            get { return Class.NotIsNull() ? Class.Code : null; }
        }
        /// <summary> Изменяемые параметры инструмента </summary>
        public Param Params = new Param();
        /// <summary> Последняя сделка по инструменту </summary>
        public Trade LastTrade = null;
        /// <summary> Возвращает последнюю цену</summary>
        public decimal LastPrice
        {
            get
            {
                if (this.LastTrade.NotIsNull()) return this.LastTrade.Price;
                if (this.Params.LastPrice > 0) return this.Params.LastPrice;
                if (this.Params.PriceClose > 0) return this.Params.PriceClose;
                return 0;
            }
        }
        /// <summary> Минимальный шаг цены </summary>
        public decimal MinPriceStep { get { return this.Params.MinPriceStep; } }
        /// <summary> Стоимость шага цены </summary>
        public decimal StepPrice { get { return this.Params.StepPrice; } }
        /// <summary> Точность (количество значащих цифр после запятой) (NUMBER) </summary>
        public int Scale = 0;
        /// <summary> Размер лота  (NUMBER)  </summary>
        public int Lot = 0;

        private readonly object objSync = new object();
        /// <summary>
        /// Стакан
        /// </summary>
        private Quote Quote = null;
        /// <summary>
        /// Задать стакан
        /// </summary>
        /// <param name="quote"></param>
        public void SetQuote(Quote quote)
        {
            lock (objSync)
            {
                Quote = quote;
            }
        }
        
        public Quote GetQuote()
        {
            lock (objSync)
            {
                return Quote.IsNull() ? null : Quote.Clone();
            }
        }
        /// <summary> Представление инструмента в виде строки </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Code + ":" + Class.Code;
        }
        /// <summary>
        /// Класс параметров инструмента
        /// </summary>
        public class Param
        {
            /// <summary> NUMERIC Лучшая цена спроса </summary>
            public decimal Bid = 0;
            /// <summary> NUMERIC Лучшая цена предложения </summary>
            public decimal Ask = 0;
            /// <summary> NUMERIC  Спрос по лучшей цене </summary>
            public decimal BidDepth = 0;
            /// <summary> NUMERIC  Предложение по лучшей цене </summary>
            public decimal AskDepth = 0;

            /// <summary> NUMERIC  Суммарный спрос </summary>
            public decimal SumBidDepth = 0;
            /// <summary> NUMERIC  Суммарное предложение </summary>
            public decimal SumAskDepth = 0;

            /// <summary> NUMERIC  Количество заявок на покупку  </summary>
            public long NumBid = 0;
            /// <summary> NUMERIC  Количество заявок на продажу  </summary>
            public long NumAsk = 0;

            /// <summary> NUMERIC Гарантийное обеспечение покупателя  </summary>
            public decimal BuyDepo = 0;
            /// <summary> NUMERIC Гарантийное обеспечение продавца  </summary>
            public decimal SellDepo = 0;

            /// <summary> STRING   Статус </summary>
            public int Status = 0;

            /// <summary> Номинал(NUMBER) </summary>
            public decimal FaceValue = 0;   //
            /// <summary> Валюта номинала  (STRING)   </summary>
            public string FaceUnit = "";  //      
            /// <summary> Минимальный шаг цены  (NUMBER) </summary>
            public decimal MinPriceStep = 0;//
            /// <summary> Дата погашения  (NUMBER)  </summary>
            public long MatDate = 0;     // 
            /// <summary> STRING   Тип инструмента </summary>
            public string SecType = "";   //
            /// <summary> DOUBLE   Число дней до погашения </summary>
            public int DaysToMatDate = -1;

            /// <summary> STRING   Время последней сделки  </summary>
            public TimeSpan TimeLastTrade;
            /// <summary> NUMERIC  Количество бумаг в последней сделке  </summary>
            public int VolumeLastTrade;
            /// <summary> NUMERIC  Цена последней сделки </summary>
            public decimal LastPrice;

            /// <summary> NUMERIC Минимальная цена сделки </summary>
            public decimal Low;
            /// <summary> NUMERIC  Максимальная цена сделки </summary>
            public decimal High;
            /// <summary> NUMERIC  Цена открытия </summary>
            public decimal Open;
            /// <summary> NUMERIC  Оборот в деньгах  </summary>
            public decimal ValToday;
            /// <summary> STRING   Состояние сессии  </summary>
            public int TradingStatus;

            /// <summary> NUMERIC  Количество сделок за сегодня </summary>
            public long NumTrades;
            /// <summary> NUMERIC Цена закрытия </summary>
            public decimal PriceClose;
            /// <summary> Date   Дата торгов  </summary>
            public DateMarket TradeDate;
            /// <summary> NUMERIC Доходность последней сделки  </summary>
            public decimal Yield;
            /// <summary> NUMERIC Стоимость одного шага. </summary>
            public decimal StepPrice;
            /// <summary> NUMERIC Минимально возможная цена. </summary>
            public decimal MinPrice;
            /// <summary> NUMERIC Максимально возможная цена. </summary>
            public decimal MaxPrice;
            /// <summary>
            /// ISIN
            /// </summary>
            public string ISIN;
        };

        /// <summary>
        /// Список параметров инструмента, которые будут обновляться
        /// </summary>
        public static string[] ListAllParams = {
//BASE
            "CODE", 			    //STRING Код бумаги  (Всегда должен идти первым)
            "CLASS_CODE", 			//STRING Код класса (Всегда должен идти вторым)
            "SEC_SCALE", 			//DOUBLE Точность цены 
//			"LONGNAME", 			//STRING Полное название бумаги 
//		    "SHORTNAME", 			//STRING Краткое название бумаги 
			"STEPPRICE", 			//NUMERIC Стоимость шага цены (для новых контрактов FORTS и RTS Standard) 
            "STATUS", 				//STRING Статус 
			"EXPDATE",				//NUMERIC Дата исполнения инструмента
//			"SEC_PRICE_STEP", 		//DOUBLE Минимальный шаг цены 
//			"LOTSIZE", 				//NUMERIC Размер лота 
//SECONDARY
//			"TRADE_DATE_CODE", 		//DOUBLE Дата торгов 
		    "MAT_DATE", 			//DOUBLE Дата погашения 
		    "DAYS_TO_MAT_DATE", 	//DOUBLE Число дней до погашения 
		    "SEC_FACE_VALUE", 		//DOUBLE Номинал бумаги 
//		    "SEC_FACE_UNIT", 		//STRING Валюта номинала

			"SECTYPE", 			    //STRING Тип инструмента
		    "PRICEMAX", 			//NUMERIC Максимально возможная цена 
		    "PRICEMIN", 			//NUMERIC Минимально возможная цена 
//		    "NUMCONTRACTS", 		//NUMERIC Количество открытых позиций 

			"BUYDEPO", 			    //NUMERIC Гарантийное обеспечение продавца 
		    "SELLDEPO", 			//NUMERIC Гарантийное обеспечение покупателя 
            "LAST", 				//NUMERIC Цена последней сделки 
			////////////////////////////////////////////
//		    "BID", 					//NUMERIC Лучшая цена спроса 
//          "OFFER", 				//NUMERIC Лучшая цена предложения 
//		    "BIDDEPTH", 			//NUMERIC Спрос по лучшей цене 
		    
		    "NUMBIDS", 				//NUMERIC Количество заявок на покупку 
            "NUMOFFERS", 			//NUMERIC Количество заявок на продажу 
            "BIDDEPTHT", 			//NUMERIC Суммарный спрос 
		    "OFFERDEPTHT", 			//NUMERIC Суммарное предложение 
		    
//		    "OFFERDEPTH", 			//NUMERIC Предложение по лучшей цене 
		    
		    "OPEN", 				//NUMERIC Цена открытия 
		    "HIGH", 				//NUMERIC Максимальная цена сделки 
		    "LOW", 					//NUMERIC Минимальная цена сделки 
//		    "CHANGE", 				//NUMERIC Разница цены последней к предыдущей сессии 
//		    "QTY", 					//NUMERIC Количество бумаг в последней сделке 
		    "TIME", 				//STRING Время последней сделки 
//		    "VOLTODAY", 			//NUMERIC Количество бумаг в обезличенных сделках 
		    "VALTODAY", 			//NUMERIC Оборот в деньгах 
		    "TRADINGSTATUS", 		//STRING Состояние сессии 
//		    "VALUE", 				//NUMERIC Оборот в деньгах последней сделки 
//		    "WAPRICE", 				//NUMERIC Средневзвешенная цена 
//		    "HIGHBID", 				//NUMERIC Лучшая цена спроса сегодня 
//		    "LOWOFFER", 			//NUMERIC Лучшая цена предложения сегодня 
//		    "NUMTRADES", 			//NUMERIC Количество сделок за сегодня 
		    "PREVPRICE", 			//NUMERIC Цена закрытия 
		    "PREVWAPRICE", 			//NUMERIC Предыдущая оценка 
//		    "CLOSEPRICE", 			//NUMERIC Цена периода закрытия 
//		    "LASTCHANGE", 			//NUMERIC % изменения от закрытия 
//		    "PRIMARYDIST", 			//STRING Размещение 
		    
            "ACCRUEDINT", 			//NUMERIC Накопленный купонный доход 
			"COUPONVALUE", 			 //NUMERIC Размер купона 		
//		    "YIELD", 			    //NUMERIC Доходность последней сделки 
		    
//		    "CHANGETIME", 			//STRING Время последнего изменения 
//            "TRADECHANGE", 			//NUMERIC Разница цены последней к предыдущей сделки (FORTS, ФБ СПБ, СПВБ) 
		    "FACEVALUE", 			//NUMERIC Номинал (для бумаг СПВБ) 
//          "MARKETPRICETODAY", 	//NUMERIC Рыночная цена 
//		    "BUYBACKPRICE", 		//NUMERIC Цена оферты 
//		    "BUYBACKDATE", 			//NUMERIC Дата оферты 
//		    "ISSUESIZE", 			//NUMERIC Объем обращения 
//		    "PREVDATE", 			//NUMERIC Дата предыдущего торгового дня 
//          "LOPENPRICE", 			//NUMERIC Официальная цена открытия 
//		    "LCURRENTPRICE", 		//NUMERIC Официальная текущая цена 
//		    "LCLOSEPRICE", 			//NUMERIC Официальная цена закрытия 
//		    "ISPERCENT", 			//STRING Тип цены фьючерса 
//		    "CLSTATE", 			    //STRING Статус клиринга 
//		    "CLPRICE", 			    //NUMERIC Котировка последнего клиринга 
//		    "STARTTIME", 			//STRING Начало основной сессии 
//		    "ENDTIME", 			    //STRING Окончание основной сессии 
//		    "EVNSTARTTIME", 		//STRING Начало вечерней сессии 
//		    "EVNENDTIME", 			//STRING Окончание вечерней сессии 
//          "CURSTEPPRICE", 		//STRING Валюта шага цены 
//		    "REALVMPRICE", 			//NUMERIC Текущая рыночная котировка 
		    "MARG", 			    //STRING Маржируемый 

		    
            //////////////////////////////////////////////////
//		    "CLOSETIME", 			 //STRING Время закрытия предыдущих торгов (для индексов РТС) 
//		    "OPENVAL", 			    //NUMERIC Значение индекса РТС на момент открытия торгов 
//		    "CHNGOPEN", 			 //NUMERIC Изменение текущего индекса РТС по сравнению со значением открытия 
//		    "CHNGCLOSE", 			 //NUMERIC Изменение текущего индекса РТС по сравнению со значением закрытия 
    //		"YIELDATPREVWAPRICE", 			 //NUMERIC Доходность по предыдущей оценке 
    //		"YIELDATWAPRICE", 			 //NUMERIC Доходность по оценке 
    //		"PRICEMINUSPREVWAPRICE", 			 //NUMERIC Разница цены последней к предыдущей оценке 
    //		"CLOSEYIELD", 			 //NUMERIC Доходность закрытия 
    //		"CURRENTVALUE", 			 //NUMERIC Текущее значение индексов Московской Биржи 
    //		"LASTVALUE", 			 //NUMERIC Значение индексов Московской Биржи на закрытие предыдущего дня 
    //		"LASTTOPREVSTLPRC", 			 //NUMERIC Разница цены последней к предыдущей сессии 
    //		"PREVSETTLEPRICE", 			 //NUMERIC Предыдущая расчетная цена 
    //		"PRICEMVTLIMIT", 			 //NUMERIC Лимит изменения цены 
    //		"PRICEMVTLIMITT1", 			 //NUMERIC Лимит изменения цены T1 
    //		"MAXOUTVOLUME", 			 //NUMERIC Лимит объема активных заявок (в контрактах) 

    //		"NEGVALTODAY", 			 //NUMERIC Оборот внесистемных в деньгах 
    //		"NEGNUMTRADES", 			 //NUMERIC Количество внесистемных сделок за сегодня 

    //		"SELLPROFIT", 			 //NUMERIC Доходность продажи 
    //		"BUYPROFIT", 			 //NUMERIC Доходность покупки 

    //		"MARKETPRICE", 			 //NUMERIC Рыночная цена вчера 
    //		"NEXTCOUPON", 			 //NUMERIC Дата выплаты купона 

    //		"DURATION", 			 //NUMERIC Дюрация 
		    
    //		"QUOTEBASIS", 			 //STRING Тип цены 
    //		"PREVADMITTEDQUOT", 			 //NUMERIC Признаваемая котировка предыдущего дня 
    //		"LASTBID", 			 //NUMERIC Лучшая спрос на момент завершения периода торгов 
    //		"LASTOFFER", 			 //NUMERIC Лучшее предложение на момент завершения торгов 
    //		"PREVLEGALCLOSEPR", 			 //NUMERIC Цена закрытия предыдущего дня 
    //		"COUPONPERIOD", 			 //NUMERIC Длительность купона 
    //		"MARKETPRICE2", 			 //NUMERIC Рыночная цена 2 
    //		"ADMITTEDQUOTE", 			 //NUMERIC Признаваемая котировка 
    //		"BGOP", 			 //NUMERIC БГО по покрытым позициям 
    //		"BGONP", 			 //NUMERIC БГО по непокрытым позициям 
    //		"STRIKE", 			 //NUMERIC Цена страйк 
    //		"STEPPRICET", 			 //NUMERIC Стоимость шага цены 
    //		"STEPPRICE", 			 //NUMERIC Стоимость шага цены (для новых контрактов FORTS и RTS Standard) 
    //		"SETTLEPRICE", 			 //NUMERIC Расчетная цена 
    //		"OPTIONTYPE", 			 //STRING Тип опциона 
    //		"OPTIONBASE", 			 //STRING Базовый актив 
    //		"VOLATILITY", 			 //NUMERIC Волатильность опциона 
    //		"THEORPRICE", 			 //NUMERIC Теоретическая цена 
    //		"PERCENTRATE", 			 //NUMERIC Агрегированная ставка 
 
//		    "MONSTARTTIME", 			 //STRING Начало утренней сессии 
//		    "MONENDTIME", 			 //STRING Окончание утренней сессии 
    //		"CROSSRATE", 			 //NUMERIC Курс 
    //		"BASEPRICE", 			 //NUMERIC Базовый курс 
    //		"HIGHVAL", 			 //NUMERIC Максимальное значение (RTSIND) 
    //		"LOWVAL", 			 //NUMERIC Минимальное значение (RTSIND) 
    //		"ICHANGE", 			 //NUMERIC Изменение (RTSIND) 
    //		"IOPEN", 			 //NUMERIC Значение на момент открытия (RTSIND) 
    //		"PCHANGE", 			 //NUMERIC Процент изменения (RTSIND) 
    //		"OPENPERIODPRICE", 			 //NUMERIC Цена предторгового периода 
    //		"MIN_CURR_LAST", 			 //NUMERIC Минимальная текущая цена 
    //		"SETTLECODE", 			 //STRING Код расчетов по умолчанию 
    //		"STEPPRICECL", 			 //DOUBLE Стоимость шага цены для клиринга 
    //		"STEPPRICEPRCL", 			 //DOUBLE Стоимость шага цены для промклиринга 
    //		"MIN_CURR_LAST_TI", 			 //STRING Время изменения минимальной текущей цены 
    //		"PREVLOTSIZE", 			 //DOUBLE Предыдущее значение размера лота 
    //		"LOTSIZECHANGEDAT", 			 //DOUBLE Дата последнего изменения размера лота 
    //		"CLOSING_AUCTION_PRICE", 			 //NUMERIC Цена послеторгового аукциона 
    //		"CLOSING_AUCTION_VOLUME", 			 //NUMERIC Количество в сделках послеторгового аукциона 
        };
    }
}
