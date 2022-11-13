using System;
using System.Collections.Generic;
using System.Linq;
using MarketObjects;
using System.Threading;
using QuikConnector.Components.Terminal;
using QuikConnector.Components.Settings;
using QuikConnector.Components.Log;
using QuikConnector.Components.Controllers;

namespace QuikConnector
{
    public class Connector
    {
        const string DEFAULT_HOST = "localhost";
        /// <summary> Настройки из конфигурационного файла </summary>
        public static Settings ConfSettings = new Settings();

        /// <summary> Контролер терминала </summary>
        private TerminalControl ConTerminal;

        /// <summary> Параметры и объекты терминала </summary>
        public TerminalControl Objects
        {
            get { return this.ConTerminal; }
        }

        /// <summary>
        /// Создает объект для подключения к терминалу.
        /// </summary>
        /// <param name="serverAddr">Адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        public Connector(string serverAddr = DEFAULT_HOST)
        {

            var pSend = ConfSettings.GetParam("Main", "TransportPort").Value.ToInt32();
            var pReceive = ConfSettings.GetParam("Main", "TransportPortReceive").Value.ToInt32();

            this.ConTerminal = new TerminalControl(serverAddr, pSend, pReceive);

            var param = Connector.ConfSettings.GetParam("SettingForStock", "MarketClass");
            //var ClassForStock = param.NotIsNull() ? param.Value : null;
            //ServiceConvertorMsg.CodesClassForStock = ClassForStock;
        }

        /// <summary> Подключиться к терминалу </summary>
        public void Connect()
        {
            this.ConTerminal.CreateSockets();
        }
        /// <summary>
        /// Посылает сигнал серверу на для проверки соединения
        /// </summary>
        public void PingServer()
        {
            this.ConTerminal.SendMsgToServer("Ping", new string[] { "1" });
        }

        /// <summary> Разорвать соединение  </summary>
        public void Disconnect()
        {
            this.ConTerminal.SendMsgToServer("Disconnect", new string[] { "1" });
            Thread.Sleep(1000);
            this.ConTerminal.CloseSockets();
        }
        /// <summary>
        /// Запрашивает порцию исторических сделок
        /// </summary>
        public void GetPortionHistoryTrades(int portion = 10000)
        {
            ConTerminal.SendMsgToServer("GetPortionTrades", new string[] { portion.ToString() });
        }

        /// <summary> Регистрирует инструмент для получения данных о нем. </summary>
        public void RegisterSecurities(string SecCode, string SecClassCode)
        {
            this.ConTerminal.SendMsgToServer("RegSec", new string[] { SecCode, SecClassCode });
        }
        /// <summary> Регистрирует инструмент для получения данных о нем. </summary>
        public void RegisterSecurities(Securities sec)
        {
            this.ConTerminal.SendMsgToServer("RegSec", new string[] { sec.Code, sec.Class.Code });
        }
        /// <summary> Регистрирует стакан инструмента для получения данных стакана. </summary>
        public void RegisterDepth(string SecCode, string SecClassCode)
        {
            this.ConTerminal.SendMsgToServer("RegDepthSec", new string[] { SecCode, SecClassCode });
        }
        /// <summary> Регистрирует стакан инструмента для получения данных стакана. </summary>
        public void RegisterDepth(Securities sec)
        {
            if (!sec.Empty())
                this.ConTerminal.SendMsgToServer("RegDepthSec", new string[] { sec.Code, sec.Class.Code });
        }
        /// <summary> Снять регистрацию стакана инструмента для отмены получения данных стакана. </summary>
        public void UnregisterDepth(string SecCode, string SecClassCode)
        {
            this.ConTerminal.SendMsgToServer("UnRegDepthSec", new string[] { SecCode, SecClassCode });
        }
        /// <summary> Снять регистрацию стакана инструмента для отмены получения данных стакана. </summary>
        public void UnregisterDepth(Securities sec)
        {
            if (!sec.Empty())
                this.ConTerminal.SendMsgToServer("UnRegDepthSec", new string[] { sec.Code, sec.Class.Code });
        }

        /// <summary>       
        /// Регистрирует параметры инструмента
        /// </summary>
        /// <param name="arrayParam"></param>
		public void RegisterParamSec(string[] arrayParam)
        {
            this.ConTerminal.SendMsgToServer("RegParamsSec", arrayParam);
        }
        /// <summary> Регистрирует все доступные параметры для инструментов которые будут обновляться. </summary>
        public void RegisterAllParamSec()
        {
            if (Securities.ListAllParams.Length > 0)
            {
                this.RegisterParamSec(Securities.ListAllParams.ToArray());
            }
        }


        /// <summary>
        /// Отправка транзакции
        /// </summary>
        /// <param name="ArrayParams"> Параметры необходимые для транзакции </param>
        /// <returns></returns>
        private int SendTransaction(string[] ArrayParams)
        {
            int count = ArrayParams.Length;
            if (count > 0)
            {
                this.ConTerminal.SendMsgToServer("Transaction", new string[] { count.ToString() }.Concat(ArrayParams).ToArray());
            }
            return 0;
        }
        /// <summary>
        /// Создает заявку.
        /// </summary>
        /// <param name="createOrder"></param>
        /// <returns></returns>
        public int CreateOrder(Order createOrder)
        {
            if (createOrder.Sec.Empty()) return -1;
            if (createOrder.Price <= 0) return -2;
            if (createOrder.Volume <= 0) return -3;
            ThreadsController.Thread(() =>
            {
                QLog.CatchException(() =>
                {
                    if (ConTerminal.tAccounts.Count > 0)
                    {
                        Account acc = this.ConTerminal.tAccounts.ToArray().FirstOrDefault(a => !a.AccClasses.FirstOrDefault(c => c.Code == createOrder.Sec.Class.Code).Empty());
                        if (acc.Empty()) return;
                        Random rnd = new Random();

                        var codeClient = createOrder.ClientCode.Empty() ? "" : createOrder.ClientCode + '/';

                        string[] Params = {
                            "TRANS_ID",     rnd.Next(1, 1000000).ToString(),
                            "ACTION",       "NEW_ORDER",
                            "CLASSCODE",    createOrder.Sec.Class.Code,
                            "SECCODE",      createOrder.Sec.Code,
                            "OPERATION",    createOrder.IsBuy() ? "B" : "S",
                            "TYPE",         "L",
                            "PRICE",        createOrder.Price.ToString(),
                            "QUANTITY",     createOrder.Volume.ToString(),
                            "CLIENT_CODE",  codeClient + (createOrder.Comment.Empty() ? "" : "/" + createOrder.Comment),
                            "ACCOUNT",      acc.AccID,
							//"COMMENT",      createOrder.Comment
						};
                        this.SendTransaction(Params);
                    }
                });
            });
            return 0;
        }

        /// <summary>
        /// Создает стоп-заявку.
        /// </summary>
        /// <param name="createOrder"></param>
        /// <returns></returns>
        public int CreateStopOrder(StopOrder createOrder, StopOrderType type)
        {
            if (createOrder.Sec.Empty()) return -1;
            if (createOrder.Price <= 0) return -2;
            if (createOrder.Volume <= 0) return -3;
            ThreadsController.Thread(() =>
            {
                QLog.CatchException(() =>
                {
                    Account acc = this.ConTerminal.tAccounts.ToArray().FirstOrDefault(a => a.AccClasses.FirstOrDefault(c => c.Code == createOrder.Sec.Class.Code) != null);
                    if (acc.Empty()) return;
                    Random rnd = new Random();
                    string TypeSOrder = "";
                    string[] DopParam = { "TRANS_ID", rnd.Next(1, 1000000).ToString() };

                    string condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();

                    switch (type)
                    {
                        case StopOrderType.StopLimit:
                            TypeSOrder = "SIMPLE_STOP_ORDER";
                            if (createOrder.IsBuy()) condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();
                            else condition = ((int)ConditionStopOrder.LessOrEqual).ToString();
                            var dateExpiry = createOrder.DateExpiry.To_YYYYMMDD();
                            DopParam = DopParam.Concat(new string[] {
                                "CONDITION", condition,
                                "EXPIRY_DATE", (dateExpiry.Empty() ? "TODAY" : dateExpiry),
                                "STOPPRICE",    Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),

                            }).ToArray();
                            break;
                        case StopOrderType.LinkOrder:
                            TypeSOrder = "WITH_LINKED_LIMIT_ORDER";
                            if (createOrder.IsBuy()) condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();
                            else condition = ((int)ConditionStopOrder.LessOrEqual).ToString();
                            DopParam = DopParam.Concat(new string[] {
                                "CONDITION",        condition,
                                "EXPIRY_DATE",      "TODAY",
                                "STOPPRICE",        Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),
                                "LINKED_ORDER_PRICE", Math.Round(createOrder.LinkOrderPrice, (int)createOrder.Sec.Scale).ToString(),
                                "KILL_IF_LINKED_ORDER_PARTLY_FILLED","NO"
                            }).ToArray();

                            break;
                        case StopOrderType.TakeProfit:
                            TypeSOrder = "TAKE_PROFIT_STOP_ORDER";
                            if (createOrder.IsBuy()) condition = ((int)ConditionStopOrder.LessOrEqual).ToString();
                            else condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();
                            DopParam = DopParam.Concat(new string[] {
                                "CONDITION", condition,
                                "EXPIRY_DATE", "TODAY",
                                "OFFSET",       Math.Round(createOrder.Offset, (int)createOrder.Sec.Scale).ToString(),
                                "OFFSET_UNITS", "PRICE_UNITS",
                                "SPREAD",       Math.Round(createOrder.Spread, (int)createOrder.Sec.Scale).ToString(),
                                "SPREAD_UNITS", "PRICE_UNITS",
                                "STOPPRICE",    Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),
                            }).ToArray();
                            break;
                        case StopOrderType.TakeProfitStopLimit:
                            TypeSOrder = "TAKE_PROFIT_AND_STOP_LIMIT_ORDER";
                            DopParam = DopParam.Concat(new string[] {
                                "OFFSET",       Math.Round(createOrder.Offset, (int)createOrder.Sec.Scale).ToString(),
                                "OFFSET_UNITS", "PRICE_UNITS",
                                "SPREAD",       Math.Round(createOrder.Spread, (int)createOrder.Sec.Scale).ToString(),
                                "SPREAD_UNITS", "PRICE_UNITS",
                                "STOPPRICE",    Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),
                                "STOPPRICE2",   Math.Round(createOrder.ConditionPrice2, (int)createOrder.Sec.Scale).ToString(),
                                "IS_ACTIVE_IN_TIME", "NO"
                            }).ToArray();
                            break;
                    };
                    string[] Params = {
                        "ACTION",       "NEW_STOP_ORDER",
                        "CLASSCODE",    createOrder.Sec.Class.Code,
                        "SECCODE",      createOrder.Sec.Code,
                        "OPERATION",    createOrder.IsBuy() ? "B" : "S",
                        "TYPE",         "L",
                        "PRICE",        Math.Round(createOrder.Price, (int)createOrder.Sec.Scale).ToString(),
                        "QUANTITY",     createOrder.Volume.ToString(),
                        "ACCOUNT",      acc.AccID,
                        "STOP_ORDER_KIND", TypeSOrder,
                        "CLIENT_CODE",  (createOrder.ClientCode.Empty() ? "" : createOrder.ClientCode + '/') + (createOrder.Comment.Empty() ? "" : "/" + createOrder.Comment),
                        "COMMENT", createOrder.Comment
                    };
                    Params = Params.Concat(DopParam).ToArray();
                    this.SendTransaction(Params);
                });
            });
            return 0;
        }
        /// <summary>
        /// Отменяет указанную заявку.
        /// </summary>
        public int CancelOrder(Order order)
        {
            if (order.IsNull()) return -1;
            return this.CancelOrder(order.Sec, order.OrderNumber);
        }

        /// <summary>
        /// Отменяет указанную заявку.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="OrderNumber">Номер заявки</param>
        /// <returns></returns>
        public int CancelOrder(Securities sec, decimal OrderNumber)
        {
            if (sec.Empty()) return -1;
            if (OrderNumber <= 0) return -2;
            ThreadsController.Thread(() =>
            {
                QLog.CatchException(() =>
                {
                    Account acc = this.ConTerminal.tAccounts.ToArray().FirstOrDefault(a => !a.AccClasses.FirstOrDefault(c => c.Code == sec.Class.Code).Empty());
                    if (acc.Empty()) return;
                    Random rnd = new Random();
                    //"|CLASSCODE|QJSIM|SECCODE|SBER|ORDER_KEY|3181375550|ACCOUNT|NL0011100043";
                    string[] Params = {
                            "TRANS_ID",     rnd.Next(1, 1000000).ToString(),
                            "ACTION",       "KILL_ORDER",
                            "CLASSCODE",    sec.Class.Code,
                            "SECCODE",      sec.Code,
                            "ACCOUNT",      acc.AccID,
                            "ORDER_KEY",    OrderNumber.ToString()
                        };
                    this.SendTransaction(Params);
                });
            });
            return 0;
        }
        /// <summary>
        /// Отменяет список переданных стоп-ордеров
        /// </summary>
        /// <param name="sOrders"></param>
        /// <returns></returns>
        public bool CancelListStopOrders(IEnumerable<StopOrder> sOrders)
        {
            if (sOrders.IsNull()) return false;
            foreach (var so in sOrders.ToArray())
            {
                this.CancelStopOrder(so.Sec, so.OrderNumber);
            }
            return true;
        }

        public int CancelStopOrder(StopOrder sOrder)
        {
            if (sOrder.NotIsNull())
            {
                if (sOrder.Sec.NotIsNull() && sOrder.OrderNumber > 0)
                {
                    return this.CancelStopOrder(sOrder.Sec, sOrder.OrderNumber);
                }
            }
            return 0;
        }

        /// <summary>
        /// Отменяет указанную стоп-заявку.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="OrderNumber">Номер заявки</param>
        /// <returns></returns>
        public int CancelStopOrder(Securities sec, decimal OrderNumber)
        {
            if (sec.Empty()) return -1;
            if (OrderNumber <= 0) return -2;
            ThreadsController.Thread(() =>
            {
                QLog.CatchException(() =>
                {
                    Account acc = this.ConTerminal.tAccounts.ToArray().FirstOrDefault(a => !a.AccClasses.FirstOrDefault(c => c.Code == sec.Class.Code).Empty());
                    if (acc.Empty()) return;
                    Random rnd = new Random();
                    //"|CLASSCODE|QJSIM|SECCODE|SBER|ORDER_KEY|3181375550|ACCOUNT|NL0011100043";
                    string[] Params = {
                        "TRANS_ID",     rnd.Next(1, 1000000).ToString(),
                        "ACTION",       "KILL_STOP_ORDER",
                        "CLASSCODE",    sec.Class.Code,
                        "SECCODE",      sec.Code,
                        "ACCOUNT",      acc.AccID,
                        "STOP_ORDER_KEY",    OrderNumber.ToString()
                    };
                    this.SendTransaction(Params);
                });
            });
            return -1;
        }
        /// <summary>
        /// Снимает все заявки по инструменту
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int CancelAllOrder(Securities sec)
        {
            if (sec.Empty()) return -1;

            QLog.CatchException(() =>
            {
                IEnumerable<Order> orders = this.ConTerminal.tOrders.ToArray().Where(o => o.Sec.Code == sec.Code
                    && o.Sec.Class.Code == sec.Class.Code && o.IsActive());
                if (!orders.Empty())
                {
                    foreach (var ord in orders.ToArray())
                    {
                        this.CancelOrder(sec, ord.OrderNumber);
                    }
                }
            });
            return 0;
        }

        /// <summary>
        /// Снимает все стоп-заявки по инструменту
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int CancelAllStopOrder(Securities sec)
        {
            if (this.ConTerminal.IsNull()) return -1;
            if (sec.Empty()) return -1;

            var orders = this.ConTerminal.tStopOrders.ToArray().Where(o => o.Sec.Code == sec.Code
                && o.Sec.Class.Code == sec.Class.Code && o.IsActive());
            if (orders.NotIsNull())
            {
                int count = orders.Count();
                foreach (var ord in orders.ToArray())
                {
                    this.CancelStopOrder(sec, ord.OrderNumber);
                }
                return count;
            }
            return 0;
        }
    }
}
