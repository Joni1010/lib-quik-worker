using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketObjects;
using Events;

namespace QuikControl
{
    /// <summary>
    /// Класс рыночных инструментов
    /// </summary>
    public class MarketTools
    {
        /// <summary>
        /// Список событий, которые выполняются отложенно(При возникновении события). Прогружает события в очередях.
        /// </summary>
        public static List<ActivatorEvent> ListAllDeferredEventBase = new List<ActivatorEvent>();
        /// <summary>
        /// Генератор событий появления/изменения обьектов
        /// </summary>
		public Action GenerateAllEvent = () =>
        {
            if (ListAllDeferredEventBase.Count > 0)
            {
                foreach (var ev in ListAllDeferredEventBase)
                {
                    ev.ExecEvent();
                }
            }
        };
        /// <summary> Объект сделок </summary>
        public MarketElement<Trade> tTrades = new MarketElement<Trade>(ListAllDeferredEventBase);
        /// <summary> Объект исторических сделок </summary>
        public MarketElement<Trade> tOldTrades = new MarketElement<Trade>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект "моих" сделок </summary>
        public MarketElement<MyTrade> tMyTrades = new MarketElement<MyTrade>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект заявок </summary>
        public MarketElement<Order> tOrders = new MarketElement<Order>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект стоп-заявок </summary>
        public MarketElement<StopOrder> tStopOrders = new MarketElement<StopOrder>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект инструментов </summary>
        public MarketElement<Securities> tSecurities = new MarketElement<Securities>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект позиций </summary>
        public MarketElement<Position> tPositions = new MarketElement<Position>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект поортфелей </summary>
        public MarketElement<Portfolio> tPortfolios = new MarketElement<Portfolio>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект для стаканов </summary>
        //public ToolsQuote tQuote = new ToolsQuote();
        //public MarketElement<Quote> tQuote = new MarketElement<Quote>(ListAllDeferredEventBase);
        /// <summary> Событие изменения стакана </summary>
        //public ToolsQuote.eventQuote OnChangeQuote { set { tQuote.OnQuote += value; } }

        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект для транзакций </summary>
        public ToolsTrans tTransaction = new ToolsTrans();
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект для клиентов </summary>
        public MarketElement<Client> tClients = new MarketElement<Client>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект для фирм </summary>
        public MarketElement<Firm> tFirms = new MarketElement<Firm>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект для классов </summary>
        public MarketElement<MarketClass> tClasses = new MarketElement<MarketClass>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary> Объект для счетов </summary>
        public MarketElement<Account> tAccounts = new MarketElement<Account>(ListAllDeferredEventBase);
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Инициализация рыночных обьектов
        /// </summary>
		public void InitMarketTools()
        {
            tTrades.DisabledCollection();
            tOldTrades.DisabledCollection();

            tFirms.DisabledEvents();
            tAccounts.DisabledEvents();
            tClasses.DisabledEvents();
            tClients.DisabledEvents();

            tSecurities.OnOverFlow += GenerateAllEvent;
            tTrades.OnOverFlow += GenerateAllEvent;
            tMyTrades.OnOverFlow += GenerateAllEvent;
            tOrders.OnOverFlow += GenerateAllEvent;
            tStopOrders.OnOverFlow += GenerateAllEvent;
            //tQuote.OnOverFlow += this.GenerateAllEvent;
        }
    }
}
