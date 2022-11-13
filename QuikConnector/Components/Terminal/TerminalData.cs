using MarketObjects;
using QuikConnector.Components.Elements;

namespace QuikConnector.Components.Terminal
{
    /// <summary>
    /// Класс рыночных инструментов
    /// </summary>
    public class TerminalData
    {
        /// <summary> Объект сделок </summary>
        public Element<Trade> tTrades = new Element<Trade>(true);
        /// <summary> Объект исторических сделок </summary>
        public Element<Trade> tOldTrades = new Element<Trade>(true);
        
        /// <summary> Объект "моих" сделок </summary>
        public Element<MyTrade> tMyTrades = new Element<MyTrade>(true);
        
        /// <summary> Объект заявок </summary>
        public Element<Order> tOrders = new Element<Order>(false);
        
        /// <summary> Объект стоп-заявок </summary>
        public Element<StopOrder> tStopOrders = new Element<StopOrder>(false);
        
        /// <summary> Объект инструментов </summary>
        public Element<Securities> tSecurities = new Element<Securities>(true);
        
        /// <summary> Объект позиций </summary>
        public Element<Position> tPositions = new Element<Position>(false);
        
        /// <summary> Объект поортфелей </summary>
        public Element<Portfolio> tPortfolios = new Element<Portfolio>(false);
        

        /// <summary> Объект для стаканов </summary>
        //public ToolsQuote tQuote = new ToolsQuote();
        //public MarketElement<Quote> tQuote = new MarketElement<Quote>(ListAllDeferredEventBase);
        /// <summary> Событие изменения стакана </summary>
        //public ToolsQuote.eventQuote OnChangeQuote { set { tQuote.OnQuote += value; } }

        
        /// <summary> Объект для транзакций </summary>
        public ToolsTrans tTransaction = new ToolsTrans();
        
        /// <summary> Объект для клиентов </summary>
        public Element<Client> tClients = new Element<Client>(true);
        
        /// <summary> Объект для фирм </summary>
        public Element<Firm> tFirms = new Element<Firm>(true);
        
        /// <summary> Объект для классов </summary>
        public Element<MarketClass> tClasses = new Element<MarketClass>(true);
        
        /// <summary> Объект для счетов </summary>
        public Element<Account> tAccounts = new Element<Account>(false);

        protected void generatingElements()
        {
            tFirms.generating();
            tClients.generating();
            tClasses.generating();
            tAccounts.generating();
            tSecurities.generating();

            tMyTrades.generating();
            tOrders.generating();
            tStopOrders.generating();
            tPositions.generating();
            tPortfolios.generating();
            tTrades.generating();
            tOldTrades.generating();
        }
        
    }
}
