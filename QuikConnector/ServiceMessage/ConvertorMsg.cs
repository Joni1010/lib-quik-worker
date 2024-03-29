﻿using Connector.Logs;
using Managers;
using MarketObjects;
using QuikControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using QuikConnector.MarketObjects;
using QuikConnector.libs;
using QuikConnector.ServiceMessage;
using System.Runtime.Serialization.Json;
using QuikConnector.MarketObjects.Structures;
using System.IO;
using System.Text;
using QuikConnector.ServiceMessage.Message;
using QuikConnector;

namespace ServiceMessage
{
    public class ConvertorMsg
    {
        public delegate void EventStartMarket();
        /// <summary>
        /// Событие начала поступления рыночных данных
        /// </summary>
		public event EventStartMarket OnStartMarket;

        public QControlTerminal Trader = null;

        /// <summary> Список рыночных классов </summary>
        private List<string> MarketClasses = new List<string>();

        public ConvertorMsg(QControlTerminal trader)
        {
            Trader = trader;
        }

        /// <summary>
        /// Обработчик новых сообщений
        /// </summary>
        /// <param name="msg"></param>
        public MsgReport? NewMessage(Msg msg)
        {
            MsgReport? report = null;

            switch (msg.Code())
            {
                case MsgCodes.CODE_MSG_TYPE_SERVICE:
                    report = GetTerminalInfo(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_START_TRADES:
                    break;

                case MsgCodes.CODE_MSG_TYPE_FIRM:
                    report = GetFirmFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_CLASS:
                    report = GetClassFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_ACCOUNT:
                    report = GetAccountsFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_CLIENT:
                    report = GetClientsFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_SECURITIES:
                    report = GetSecuritiesFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_CHECKSECURITIES:
                    report = GetCheckCountSec(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_DEPOLIMIT:
                    report = GetDepoLimitFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_MONEYLIMIT:
                    report = GetMoneyLimitsFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_FUTURESHOLDING:
                    report = GetFuturesHoldingFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_FUTURESLIMIT:
                    report = GetFutLimitsFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_ORDER:
                    report = GetOrderFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_STOPORDER:
                    report = GetStopOrderFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_MYTRADE:
                    report = GetMyTradeFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_TRADE:
                    report = GetTradeFromArrayMsg(msg.Content());
                    break;
                case MsgCodes.CODE_MSG_TYPE_TRADE_HISTORY:
                    report = GetTradeFromArrayMsg(msg.Content(), false);
                    break;
                default:
                    Qlog.Write("Fail msg: " + msg);
                    break;
            }
            /*
            switch (msg.Content())
            {
                case "OnQuote":
                    report = GetQuoteFromArrayMsg(msg);
                    break;
                case "OnChangeSecurities":
                    report = GetChangeSecuritiesFromArrayMsg(msg, false);
                    break;
                /////////////////////////////////
                case "OnPortfolioInfo":
                    report = GetPortfoliosFromArrayMsg(msg);
                    break;
                case "OnTransReply":
                    report = GetTransReplyFromArrayMsg(msg);
                    break;
            }*/
            return report;
        }

        /// <summary> Отправка флага что все базовые данные загружены </summary>
        /// <param name="msg"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public MsgReport? GetCheckCountSec(string msg)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(msg));
            var json = new DataContractJsonSerializer(typeof(SCheckSec));
            var sCheckSec = json.ReadObject(ms) as SCheckSec;
            ms.Close();

            MThread.InitThread(() =>
            {
                var countGotSec = sCheckSec.count_sec.ToInt32();
                int timeOut = 2000;
                while (MManager.LoopProcessing)
                {
                    QDebug.write(Trader.tSecurities.Count.ToString());

                    if (countGotSec == Trader.tSecurities.Count)
                    {
                        //Базовые данные загружены
                        break;
                    }
                    timeOut--;
                    if (timeOut == 0)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }
                if (timeOut == 0)
                {
                    Qlog.Write("Error load securities:" + countGotSec + "/" + Trader.tSecurities.Count);
                }
            });
            return new MsgReport()
            {
                Object = new object(),
                Reply = string.Join(MsgServer.SP_FORSERVER.ToString(), new string[] { "start_market", "1" })
            };
        }


        /// <summary> Список активных инструментов </summary>
        private List<Securities> ActiveSec = new List<Securities>();

        /// <summary> Поиск инструмента среди активных, если не найден то добавляет его в активные. <summary>
        /// <param name="CodeSec"> Код инструмента</param>
        /// <param name="classCode">Код класса инструмента</param>
        /// <returns> Объекс Security, иначе null.</returns>
        public Securities FindSecurities(string CodeSec, string classCode)
        {
            Securities LastSecFind = ActiveSec.FirstOrDefault(s => s.Code == CodeSec && classCode.Contains(s.Class.Code));
            if (LastSecFind.IsNull())
            {
                LastSecFind = Trader.tSecurities.SearchFirst(s => s.Code == CodeSec && classCode.Contains(s.Class.Code));
                if (LastSecFind.IsNull()) return null;
                ActiveSec.Add(LastSecFind);
                return LastSecFind;
            }
            else return LastSecFind;
        }
        /// <summary>
        /// Поиск инструмента среди активных, если не найден то добавляет его в активные.
        /// </summary>
        /// <param name="CodeSec">Код инструмента</param>
        /// <returns>Объекс Security, иначе null.</returns>
        public Securities FindSecurities(string CodeSec)
        {
            Securities LastSecFind = ActiveSec.FirstOrDefault(s => s.Code == CodeSec);
            if (LastSecFind.IsNull())
            {
                LastSecFind = Trader.tSecurities.SearchFirst(s => s.Code == CodeSec);
                if (LastSecFind.IsNull()) return null;
                ActiveSec.Add(LastSecFind);
                return LastSecFind;
            }
            else return LastSecFind;
        }

        /// <summary> Поиск инструмента по ID cx, если не найден то добавляет его. </summary>
        /// <param name="AccID">ID аккаунта</param>
        /// <param name="SecCode">Код инструмента</param>
        /// <returns></returns>
        public Securities FindSecuritiesByAccID(string AccID, string SecCode)
        {
            var acc = Trader.tAccounts.SearchFirst(a => a.AccID == AccID);
            if (acc != null)
            {
                foreach (var cl in acc.AccClasses)
                {
                    var sec = Trader.tSecurities.SearchFirst(s => s.Code == SecCode && s.Class.Code == cl.Code);
                    if (sec != null) return sec;
                }
            }
            return null;
        }

        /// <summary> Список активных фирм </summary>
        private List<Firm> ActiveFirms = new List<Firm>();
        /// <summary> Поиск фирмы среди активных, если не найден то добавляет его. </summary>
        public Firm FindFirm(string idFirm)
        {
            Firm LastFindFirm = ActiveFirms.FirstOrDefault(f => f.Id == idFirm);
            if (LastFindFirm == null)
            {
                LastFindFirm = Trader.tFirms.SearchFirst(f => f.Id == idFirm);
                if (LastFindFirm == null) return null;
                ActiveFirms.Add(LastFindFirm);
                return LastFindFirm;
            }
            else return LastFindFirm;
        }

        /// <summary> Список активных классов </summary>
        private List<MarketClass> ActiveClasses = new List<MarketClass>();
        /// <summary> Поиск класса среди активных, если не найден то добавляет его. </summary>
        public MarketClass FindClass(string Code)
        {
            MarketClass LastFindClass = ActiveClasses.FirstOrDefault(c => c.Code == Code);
            if (LastFindClass == null)
            {
                LastFindClass = Trader.tClasses.SearchFirst(c => c.Code == Code);
                if (LastFindClass == null) return null;
                ActiveClasses.Add(LastFindClass);
                return LastFindClass;
            }
            else return LastFindClass;
        }

        /// <summary>
        /// Преобразование сообщения в класс TerminalInfo
        /// </summary>
        /// <param name="m"></param>
        /// <param name="Msg"></param>
        private MsgReport? GetTerminalInfo(string msg)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(msg));
            var json = new DataContractJsonSerializer(typeof(STerminal));
            var terminal = json.ReadObject(ms) as STerminal;
            ms.Close();

            Trader.Terminal.Version = terminal.VERSION;
            Trader.Terminal.TradeDate = Convert.ToDateTime(terminal.TRADEDATE);
            if (terminal.SERVERTIME.Length > 0)
            {
                Trader.Terminal.ServerTime = Convert.ToDateTime(terminal.SERVERTIME).TimeOfDay;
            }
            if (terminal.LASTRECORDTIME.Length > 0)
            {
                Trader.Terminal.LastRecordTime = Convert.ToDateTime(terminal.LASTRECORDTIME).TimeOfDay;
            }

            if (terminal.CONNECTION == "установлено")
            {
                Trader.Terminal.Connect = true;
            }
            else
            {
                Trader.Terminal.Connect = false;
            }

            Trader.Terminal.IpServerAddr = terminal.IPADDRESS;
            Trader.Terminal.Port = terminal.IPPORT;
            Trader.Terminal.ServerDescription = terminal.SERVER;
            Trader.Terminal.User = terminal.USER;
            Trader.Terminal.UserID = terminal.USERID;
            Trader.Terminal.Organization = terminal.ORG;
            Trader.Terminal.Localtime = Convert.ToDateTime(terminal.LOCALTIME).TimeOfDay;
            if (terminal.CONNECTIONTIME.Length > 0)
            {
                Trader.Terminal.ConnectionTime = Convert.ToDateTime(terminal.CONNECTIONTIME).TimeOfDay;
            }
            return new MsgReport()
            {
                Object = Trader.Terminal,
            };
        }

        private Securities LastSecTrade = null;
        /// <summary>
        /// Преобразование сообщения в класс Сделки
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        private MsgReport? GetTradeFromArrayMsg(string msg, bool newItem = true)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(msg));
            var json = new DataContractJsonSerializer(typeof(STrade));
            var strade = json.ReadObject(ms) as STrade;
            ms.Close();


            Trade newTrade = new Trade();
            newTrade.Number = strade.trade_num.ToLong();
            newTrade.SecCode = strade.sec_code;
            if (LastSecTrade.NotIsNull())
            {
                if (LastSecTrade.Code == newTrade.SecCode && LastSecTrade.Class.Code == strade.class_code)
                {
                    newTrade.Sec = LastSecTrade;
                }
            }
            if (newTrade.Sec.IsNull())
            {
                newTrade.Sec = FindSecurities(newTrade.SecCode, strade.class_code);
            }
            LastSecTrade = newTrade.Sec;

            if (newTrade.Sec.IsNull())
            {
                Qlog.Write("Not security trade: " + strade.sec_code + ":" + strade.class_code);
                Qlog.Write("Not security trade: " + strade.sec_code + ":" + strade.class_code);
                return null;
            }

            newTrade.Price = strade.price.ToDecimal(newTrade.Sec.Scale);
            newTrade.Volume = strade.qty.ToInt32();
            newTrade.DateTrade = strade.datetime.GetDate();
            BitArray bitsFlags = new BitArray(new int[] { strade.flags.ToInt32() });
            if (bitsFlags[0] == true)
            {
                newTrade.Direction = OrderDirection.Buy;
            }
            if (bitsFlags[1] == true)
            {
                newTrade.Direction = OrderDirection.Sell;
            }

            newTrade.OpenInterest = strade.open_interest != "" ? strade.open_interest.ToDecimal() : 0;

            if (newItem && newTrade.Number > 0)
            {
                //последняя сделка
                if (newTrade.Sec.LastTrade.IsNull())
                {
                    newTrade.Sec.LastTrade = newTrade;
                }
                else
                {
                    if (newTrade.Sec.LastTrade.Number < newTrade.Number)
                    {
                        newTrade.Sec.LastTrade = newTrade;
                    }
                }
                Trader.tTrades.Add(newTrade, false);
            }
            else
            {
                Trader.tOldTrades.Add(newTrade, false);
            }
            return new MsgReport()
            {
                Object = newTrade,
            };

            /*
            if (msg.IsNull())
            {
                return null;
            }
            // 0 event | 1 trade_num | 2 sec_code | 3 class_code | 4 price | 5 qty | 
            // 6 datetime.year | 7 datetime.month | 8 datetime.day | 9 datetime.hour | 10 datetime.min | 
            // 11 datetime.sec | 12 datetime.ms | 13 flags | 14 value | 15 open_interest 
            if (msg.Struct.Length > 5)
            {
                var classCode = msg.Struct[3];
                Trade newTrade = new Trade();
                DateMarket date = new DateMarket();

                newTrade.Number = msg.Struct[1].ToLong();
                newTrade.SecCode = msg.Struct[2];
                if (LastSecTrade.NotIsNull())
                {
                    if (LastSecTrade.Code == newTrade.SecCode && LastSecTrade.Class.Code == classCode)
                    {
                        newTrade.Sec = LastSecTrade;
                    }
                }
                if (newTrade.Sec.IsNull())
                {
                    newTrade.Sec = FindSecurities(newTrade.SecCode, classCode);
                }
                LastSecTrade = newTrade.Sec;

                if (newTrade.Sec.IsNull())
                {
                    Qlog.Write("Not security trade: " + msg.Struct[2] + ":" + msg.Struct[3]);
                    return null;
                }

                newTrade.Price = msg.Struct[4].ToDecimal(newTrade.Sec.Scale);
                newTrade.Volume = msg.Struct[5].ToInt32();

                date.SetDateTimeByStruct(6, msg.Struct);

                newTrade.DateTrade = date;
                BitArray bitsFlags = new BitArray(new int[] { msg.Struct[13].ToInt32() });

                if (bitsFlags[0] == true) newTrade.Direction = OrderDirection.Buy;
                if (bitsFlags[1] == true) newTrade.Direction = OrderDirection.Sell;

                newTrade.OpenInterest = msg.Struct[15] != "" ? msg.Struct[15].ToDecimal() : 0;

                if (newItem && newTrade.Number > 0)
                {
                    //последняя сделка
                    if (newTrade.Sec.LastTrade.IsNull())
                    {
                        newTrade.Sec.LastTrade = newTrade;
                    }
                    else
                    {
                        if (newTrade.Sec.LastTrade.Number < newTrade.Number)
                        {
                            newTrade.Sec.LastTrade = newTrade;
                        }
                    }
                    Trader.tTrades.Add(newTrade, false);
                }
                else
                {
                    Trader.tOldTrades.Add(newTrade, false);
                }
                return new Message.Report(newTrade);
            }
            return null;*/
        }

        /// <summary> Обработка сообщения со стоп заявкой. </summary>
        /// <param name="m">Сообщение</param>
        /// <param name="Msg">Сообщение в виде массива</param>
        /// <param name="Trader">Главный объект Trader</param>
        /// <returns></returns>
        private MsgReport? GetStopOrderFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SStopOrder));
            var sstopOrder = json.ReadObject(ms) as SStopOrder;
            ms.Close();

            int flags = sstopOrder.flags.ToInt32();
            StopOrder s_order = new StopOrder
            {
                uid = sstopOrder.uid != "" ? sstopOrder.uid.ToDecimal() : 0,
                SecCode = sstopOrder.sec_code
            };
            s_order.Sec = FindSecurities(sstopOrder.sec_code, sstopOrder.class_code);

            s_order.OrderNumber = sstopOrder.order_num.ToLong();
            s_order.TransID = sstopOrder.trans_id.ToLong();
            //Заявка не прошла (или отброшена из-за ненадобности)
            //if (s_order.uid == 0) return null;
            StopOrder stoporder = Trader.tStopOrders.SearchFirst(or => or.OrderNumber == s_order.OrderNumber && or.SecCode == s_order.SecCode);
            bool changeOrder = true;
            if (stoporder.IsNull())
            {
                stoporder = s_order;
                Trader.tStopOrders.Add(stoporder, false);
                changeOrder = false;
            }

            stoporder.Price = sstopOrder.price.ToDecimal(stoporder.Sec.Scale);
            stoporder.ConditionPrice = sstopOrder.condition_price.ToDecimal(stoporder.Sec.Scale);
            stoporder.ConditionPrice2 = sstopOrder.condition_price2.ToDecimal(stoporder.Sec.Scale);
            stoporder.Volume = sstopOrder.qty.ToInt32();
            stoporder.Balance = sstopOrder.balance.ToInt32();
            stoporder.Offset = sstopOrder.offset.ToDecimal();
            stoporder.LinkOrderNum = sstopOrder.co_order_num.ToLong();
            stoporder.LinkOrderPrice = sstopOrder.co_order_price.ToDecimal(stoporder.Sec.Scale);
            stoporder.OrderNumExecute = sstopOrder.linkedorder.ToLong();
            stoporder.ConditionClassCode = sstopOrder.condition_class_code;
            stoporder.ConditionSecCode = sstopOrder.condition_sec_code;
            /*if (orFind.OrderExecute != null)
            {
                decimal t = orFind.OrderExecute.OrderNumber;
            }*/
            int cond = sstopOrder.condition.ToInt32();
            if (cond == (int)ConditionStopOrder.MoreOrEqual)
            {
                stoporder.Condition = ConditionStopOrder.MoreOrEqual;
            }
            else
            {
                stoporder.Condition = ConditionStopOrder.LessOrEqual;
            }
            stoporder.Spread = sstopOrder.spread.ToDecimal(stoporder.Sec.Scale);
            stoporder.DateCreateOrder = sstopOrder.order_date_time.GetDate();

            DateMarket dateWithDr = new DateMarket();
            // dateWithDr.SetDateTimeByStruct(31, msg.Struct);
            stoporder.WithDrawTime = sstopOrder.withdraw_datetime.GetDate().GetDateTime();
            stoporder.Comment = sstopOrder.brokerref;
            stoporder.TypeStopOrder = (StopOrderType)sstopOrder.stop_order_type.ToInt32();
            stoporder.DateExpiry = DateMarket.ExtractDateTime(sstopOrder.expiry.ConvertToDateForm_YYYYMMDD());

            BitArray bitsFlags = new BitArray(new int[] { flags });
            if (bitsFlags[0] == true)
            {
                stoporder.Status = OrderStatus.ACTIVE;
            }
            else if (bitsFlags[1] == true)
            {
                stoporder.Status = OrderStatus.CLOSED;
            }
            else
            {
                stoporder.Status = OrderStatus.EXECUTED;
            }

            if (bitsFlags[2] == true)
            {
                stoporder.Direction = OrderDirection.Sell;
            }
            else
            {
                stoporder.Direction = OrderDirection.Buy;
            }

            if (changeOrder)
            {
                Trader.tStopOrders.Change(stoporder, false);
            }
            return new MsgReport() { Object = stoporder };






            /*
            if (msg.IsNull())
            {
                return null;
            }
            if (msg.Struct.Length > 5)
            {
                // 0 OnStopOrder | 1 firmid | 2 sec_code | 3 order_num |  4 client_code | 
                // 5 ordertime | 6 price | 7 qty | 8 balance | 9 condition | 10 stopflags | 
                // 11 withdraw_time | 12 offset | 13 order_date_time.week_day | 14 order_date_time.hour | 
                // 15 order_date_time.ms | 16 order_date_time.mcs | 17 order_date_time.day | 18 order_date_time.month | 
                // 19 order_date_time.sec | 20 order_date_time.year | 21 order_date_time.min | 22 class_code | 
                // 23 condition_sec_code | 24 expiry | 25 flags | 26 co_order_num | 27 linkedorder | 
                // 28 alltrade_num | 29 condition_price | 30 condition_price2 | 31 co_order_price | 
                // 32 spread | 33 withdraw_datetime.week_day | 34 withdraw_datetime.hour | 35 withdraw_datetime.ms | 
                // 36 withdraw_datetime.mcs | 37 withdraw_datetime.day | 38 withdraw_datetime.month | 
                // 39 withdraw_datetime.sec | 40 withdraw_datetime.year | 41 withdraw_datetime.min | 42 active_to_time | 
                // 43 condition_class_code | 44 condition_seccode | 45 canceled_uid | 46 active_from_time | 
                // 47 uid | 48 brokerref | 49 filled_qty | 50 trans_id | 51 stop_order_type

                var classCode = msg.Struct[20];
                /*if (!ServiceConvertorMsg.CodesClassForStock.Contains(classCode))
                    return null;*/
            /*    int flags = msg.Struct[23].ToInt32();

                StopOrder s_order = new StopOrder
                {
                    uid = msg.Struct[43] != "" ? msg.Struct[43].ToDecimal() : 0,
                    SecCode = msg.Struct[2]
                };
                s_order.Sec = FindSecurities(s_order.SecCode, classCode);

                s_order.OrderNumber = msg.Struct[3].ToLong();
                s_order.TransID = msg.Struct[46].ToLong();
                //Заявка не прошла (или отброшена из-за ненадобности)
                //if (s_order.uid == 0) return null;
                StopOrder stoporder = Trader.tStopOrders.SearchFirst(or => or.OrderNumber == s_order.OrderNumber && or.SecCode == s_order.SecCode);
                bool changeOrder = true;
                if (stoporder.IsNull())
                {
                    stoporder = s_order;
                    Trader.tStopOrders.Add(stoporder, false);
                    changeOrder = false;
                }

                stoporder.Price = msg.Struct[6].ToDecimal(stoporder.Sec.Scale);
                stoporder.ConditionPrice = msg.Struct[27].ToDecimal(stoporder.Sec.Scale);
                stoporder.ConditionPrice2 = msg.Struct[28].ToDecimal(stoporder.Sec.Scale);
                stoporder.Volume = msg.Struct[7].ToInt32();
                stoporder.Balance = msg.Struct[8].ToInt32();
                stoporder.Offset = msg.Struct[12].ToDecimal();
                stoporder.LinkOrderNum = msg.Struct[24].ToLong();
                stoporder.LinkOrderPrice = msg.Struct[29].ToDecimal(stoporder.Sec.Scale);
                stoporder.OrderNumExecute = msg.Struct[25].ToLong();
                stoporder.ConditionClassCode = msg.Struct[39];
                stoporder.ConditionSecCode = msg.Struct[40];
                /*if (orFind.OrderExecute != null)
                {
                    decimal t = orFind.OrderExecute.OrderNumber;
                }*/
            /*       int cond = msg.Struct[9].ToInt32();
                   if (cond == (int)ConditionStopOrder.MoreOrEqual)
                   {
                       stoporder.Condition = ConditionStopOrder.MoreOrEqual;
                   }
                   else
                   {
                       stoporder.Condition = ConditionStopOrder.LessOrEqual;
                   }
                   stoporder.Spread = msg.Struct[30].ToDecimal(stoporder.Sec.Scale);

                   DateMarket date = new DateMarket();
                   date.SetDateTimeByStruct(13, msg.Struct);
                   stoporder.DateCreateOrder = date;

                   DateMarket dateWithDr = new DateMarket();
                   dateWithDr.SetDateTimeByStruct(31, msg.Struct);
                   stoporder.WithDrawTime = dateWithDr.GetDateTime();
                   stoporder.Comment = msg.Struct[44];
                   stoporder.TypeStopOrder = (StopOrderType)Convert.ToInt32(msg.Struct[47]);
                   stoporder.DateExpiry = DateMarket.ExtractDateTime(msg.Struct[22].ConvertToDateForm_YYYYMMDD());

                   BitArray bitsFlags = new BitArray(new int[] { flags });
                   if (bitsFlags[0] == true)
                   {
                       stoporder.Status = OrderStatus.ACTIVE;
                   }
                   else if (bitsFlags[1] == true)
                   {
                       stoporder.Status = OrderStatus.CLOSED;
                   }
                   else
                   {
                       stoporder.Status = OrderStatus.EXECUTED;
                   }

                   if (bitsFlags[2] == true)
                   {
                       stoporder.Direction = OrderDirection.Sell;
                   }
                   else
                   {
                       stoporder.Direction = OrderDirection.Buy;
                   }

                   if (changeOrder)
                   {
                       Trader.tStopOrders.Change(stoporder, false);
                   }
                   return new Message.Report(stoporder);
               }
               return null;*/
        }
        /// <summary>
        /// Преобразование сообщения в заявку
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public MsgReport? GetOrderFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SOrder));
            var sorder = json.ReadObject(ms) as SOrder;
            ms.Close();

            Order ord = new Order();
            int flags = sorder.flags.ToInt32();
            ord.OrderNumber = sorder.order_num.ToLong();
            ord.SecCode = sorder.sec_code;
            ord.Sec = FindSecurities(ord.SecCode, sorder.class_code);
            if (ord.Sec.IsNull())
            {
                return null;
            }
            Order orFind = Trader.tOrders.SearchFirst(or => or.OrderNumber == ord.OrderNumber && or.SecCode == ord.SecCode);

            bool change = true;
            if (orFind.IsNull())
            {
                orFind = ord;
                Trader.tOrders.Add(orFind, false);
                change = false;
            }

            orFind.OrderTrades = Trader.tMyTrades.SearchAll(mt => mt.OrderNum == ord.OrderNumber);
            if (orFind.OrderTrades.NotIsNull()) orFind.OrderTrades.ToList().ForEach(t => t.Order = ord);

            orFind.Price = sorder.price.ToDecimal(orFind.Sec.Scale);
            orFind.Volume = sorder.qty.ToInt32();
            orFind.Balance = sorder.balance.ToInt32();
            orFind.Value = sorder.value.ToDecimal(2);
            orFind.DateCreateOrder = sorder.datetime.GetDate();
            orFind.Comment = sorder.brokerref;
            orFind.uid = sorder.uid.ToDecimal();
            orFind.TransID = sorder.trans_id.ToLong();

            BitArray bitsFlags = new BitArray(new int[] { flags });
            if (bitsFlags[0] == true)
            {
                orFind.Status = OrderStatus.ACTIVE;
            }
            else if (bitsFlags[1] == true)
            {
                orFind.Status = OrderStatus.CLOSED;
            }
            else
            {
                orFind.Status = OrderStatus.EXECUTED;
            }

            if (bitsFlags[2] == true)
            {
                orFind.Direction = OrderDirection.Sell;
            }
            else
            {
                orFind.Direction = OrderDirection.Buy;
            }

            if (change)
            {
                Trader.tOrders.Change(orFind, false);// !MManager.EventPackages);
            }
            return new MsgReport() { Object = orFind };

            /*
            if (msg.IsNull()) return null;

            int Count = msg.Struct.Length;
            if (Count > 5)
            {
                Order ord = new Order();
                // 0 OnOrder | 1 sec_code | 2 ordernum | 3 userid | 4 client_code | 5 firmid | 
                // 6 account | 7 flags | 8 price | 9 balance | 10 value | 11 qty | 
                // 12 year | 13 month | 14 day | 15 hour | 16 min | 17 sec | 18 ms | 
                // 19 brokerref | 20 trans_id | 21 class_code

                var classCode = msg.Struct[21];
                /*if (!ServiceConvertorMsg.CodesClassForStock.Contains(classCode))
                    return null;*/
            /*
                            string userid = msg.Struct[3];
                            string clientCode = msg.Struct[4];
                            string firmid = msg.Struct[5];
                            int flags = msg.Struct[7].ToInt32();
                            ord.OrderNumber = msg.Struct[2].ToLong();
                            ord.SecCode = msg.Struct[1];
                            ord.Sec = FindSecurities(ord.SecCode, classCode);
                            if (ord.Sec.IsNull()) return null;
                            Order orFind = Trader.tOrders.SearchFirst(or => or.OrderNumber == ord.OrderNumber && or.SecCode == ord.SecCode);

                            bool change = true;
                            if (orFind.IsNull())
                            {
                                orFind = ord;
                                Trader.tOrders.Add(orFind, false);
                                change = false;
                            }

                            orFind.OrderTrades = Trader.tMyTrades.SearchAll(mt => mt.OrderNum == ord.OrderNumber);
                            if (orFind.OrderTrades.NotIsNull()) orFind.OrderTrades.ToList().ForEach(t => t.Order = ord);

                            orFind.Price = msg.Struct[8].ToDecimal(orFind.Sec.Scale);
                            orFind.Volume = msg.Struct[11].ToInt32();
                            orFind.Balance = msg.Struct[9].ToInt32();
                            orFind.Value = msg.Struct[10].ToDecimal(2);

                            DateMarket date = new DateMarket();
                            date.SetDateTimeByStruct(12, msg.Struct);

                            orFind.DateCreateOrder = date;

                            orFind.Comment = msg.Struct[19];
                            orFind.uid = 0;
                            orFind.TransID = msg.Struct[20].ToInt64();

                            BitArray bitsFlags = new BitArray(new int[] { flags });
                            if (bitsFlags[0] == true)
                            {
                                orFind.Status = OrderStatus.ACTIVE;
                            }
                            else if (bitsFlags[1] == true)
                            {
                                orFind.Status = OrderStatus.CLOSED;
                            }
                            else
                            {
                                orFind.Status = OrderStatus.EXECUTED;
                            }

                            if (bitsFlags[2] == true)
                            {
                                orFind.Direction = OrderDirection.Sell;
                            }
                            else
                            {
                                orFind.Direction = OrderDirection.Buy;
                            }
                            //Заявка не прошла (или отброшена из-за ненадобности)
                            //if (orFind.uid == 0) return null;

                            if (change)
                            {
                                Trader.tOrders.Change(orFind, false);// !MManager.EventPackages);
                            }
                            return new Message.Report(orFind);
                        }
                        return null;*/
        }

        /// <summary>
        /// Преобразование сообщения в класс "Моя сделка"
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="MsgTrade"></param>
        /// <returns></returns>
        public MsgReport? GetMyTradeFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SMyTrade));
            var sMyTrade = json.ReadObject(ms) as SMyTrade;
            ms.Close();

            Trade trade = new Trade();
            // 0 OnMyTrade | 1 trade_num | 2 sec_code | 3 class_code | 4 price | 5 qty | 
            // 6 datetime.year | 7 datetime.month | 8 datetime.day | 9 datetime.hour | 10 datetime.min | 11 datetime.sec | 
            // 12 datetime.ms | 13 flags | 14 value | 15 order_num | 
            // 16 uid | 17 broker_comission | 18 exchange_comission | 19 clearing_comission | 20 block_securities | 21 brokerref
            trade.SecCode = sMyTrade.sec_code;
            trade.Sec = FindSecurities(trade.SecCode, sMyTrade.class_code);
            if (trade.Sec.IsNull())
            {
                //Qlog.Write("Not security my trade:" + trade.SecCode + ":" + classCode);
                return null;
            }

            trade.Number = sMyTrade.trade_num.ToLong();
            trade.Price = sMyTrade.price.ToDecimal(trade.Sec.Scale);
            trade.Volume = sMyTrade.qty.ToInt32();
            trade.DateTrade = sMyTrade.datetime.GetDate();
            /////////// Construct my trade ///////////
            MyTrade my_trade = new MyTrade(trade)
            {
                OrderNum = sMyTrade.order_num.ToLong(),
                uid = sMyTrade.uid.ToLong(),
                BrokerComission = sMyTrade.broker_comission.ToDecimal(2),
                ExchangeComission = sMyTrade.exchange_comission.ToDecimal(2),
                ClearingComission = sMyTrade.clearing_comission.ToDecimal(2),
                BlockSecurities = sMyTrade.block_securities.ToDecimal(2),
                Comment = sMyTrade.brokerref
            };

            if (my_trade.Trade.Number > 0 && my_trade.OrderNum > 0)
            {
                //из-за того что приходит 2 дубликата откидываем один.
                MyTrade mT = Trader.tMyTrades.SearchFirst(mt => mt.Trade.Number == my_trade.Trade.Number && mt.OrderNum == my_trade.OrderNum);
                if (mT.IsNull())
                {
                    //Поиск своей заявки и добавлении транзакции в список сделок заявки.
                    Order or = Trader.tOrders.SearchFirst(o => o.OrderNumber == my_trade.OrderNum);

                    if (or.NotIsNull())
                    {
                        my_trade.Trade.Direction = or.Direction;
                        or.OrderTrades = Trader.tMyTrades.SearchAll(mt => mt.OrderNum == or.OrderNumber);
                        my_trade.Order = or;
                    }
                    if (my_trade.Trade.Direction.IsNull())
                    {
                        BitArray bitsFlags = new BitArray(new int[] { sMyTrade.flags.ToInt32() });
                        if (bitsFlags[2] == true) my_trade.Trade.Direction = OrderDirection.Sell;
                        else my_trade.Trade.Direction = OrderDirection.Buy;
                    }
                }
            }
            return new MsgReport() { Object = my_trade };

            /*
            if (msg.IsNull())
            {
                return null;
            }

            int Count = msg.Struct.Length;
            if (Count > 5)
            {
                /////// Forming Trade /////////
                Trade trade = new Trade();
                // 0 OnMyTrade | 1 trade_num | 2 sec_code | 3 class_code | 4 price | 5 qty | 
                // 6 datetime.year | 7 datetime.month | 8 datetime.day | 9 datetime.hour | 10 datetime.min | 11 datetime.sec | 
                // 12 datetime.ms | 13 flags | 14 value | 15 order_num | 
                // 16 uid | 17 broker_comission | 18 exchange_comission | 19 clearing_comission | 20 block_securities | 21 brokerref
                var classCode = msg.Struct[3];
                trade.SecCode = msg.Struct[2];
                trade.Sec = FindSecurities(trade.SecCode, classCode);
                if (trade.Sec.IsNull())
                {
                    //Qlog.Write("Not security my trade:" + trade.SecCode + ":" + classCode);
                    return null;
                }

                DateMarket date = new DateMarket();
                trade.Number = msg.Struct[1].ToLong();
                trade.Price = msg.Struct[4].ToDecimal(trade.Sec.Scale);
                trade.Volume = msg.Struct[5].ToInt32();
                date.SetDateTimeByStruct(6, msg.Struct);
                trade.DateTrade = date;
                /////////// Construct my trade ///////////
                MyTrade my_trade = new MyTrade(trade)
                {
                    OrderNum = msg.Struct[15].ToLong(),
                    uid = msg.Struct[16].ToLong(),
                    BrokerComission = msg.Struct[17].ToDecimal(2),
                    ExchangeComission = msg.Struct[18].ToDecimal(2),
                    ClearingComission = msg.Struct[19].ToDecimal(2),
                    BlockSecurities = msg.Struct[20].ToDecimal(2),
                    Comment = msg.Struct[21]
                };

                if (my_trade.Trade.Number > 0 && my_trade.OrderNum > 0)
                {
                    //из-за того что приходит 2 дубликата откидываем один.
                    MyTrade mT = Trader.tMyTrades.SearchFirst(mt => mt.Trade.Number == my_trade.Trade.Number && mt.OrderNum == my_trade.OrderNum);
                    if (mT.IsNull())
                    {
                        //Поиск своей заявки и добавлении транзакции в список сделок заявки.
                        Order or = Trader.tOrders.SearchFirst(o => o.OrderNumber == my_trade.OrderNum);

                        if (or.NotIsNull())
                        {
                            my_trade.Trade.Direction = or.Direction;
                            or.OrderTrades = Trader.tMyTrades.SearchAll(mt => mt.OrderNum == or.OrderNumber);
                            my_trade.Order = or;
                        }
                        if (my_trade.Trade.Direction.IsNull())
                        {
                            BitArray bitsFlags = new BitArray(new int[] { msg.Struct[13].ToInt32() });
                            if (bitsFlags[2] == true) my_trade.Trade.Direction = OrderDirection.Sell;
                            else my_trade.Trade.Direction = OrderDirection.Buy;
                        }
                    }
                }
                return new Message.Report(my_trade);
            }
            return null;*/
        }

        /// <summary>
        /// Преобразование сообщения в класс Фирмы
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private MsgReport? GetFirmFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SFirm));
            var sfirm = json.ReadObject(ms) as SFirm;
            ms.Close();

            Firm firm = new Firm
            {
                Id = sfirm.firmid,
                Exchange = sfirm.exchange,
                Status = sfirm.status.ToInt32(),
                Name = sfirm.firm_name,
            };

            if (firm.Id.Length > 0)
            {
                Trader.tFirms.Add(firm, false);
                return new MsgReport() { Object = firm };
            }

            /*
            //0 OnFirm | 1 firmid | 2 exchange | 3 status | 4 firm_name
            if (msg.Struct.Length > 0)
            {
                Firm firm = new Firm
                {
                    Id = msg.Struct[1],
                    Exchange = msg.Struct[2],
                    Status = msg.Struct[3].ToInt32(),
                    Name = msg.Struct[4]
                };
                if (firm.Id != "")
                {
                    Trader.tFirms.Add(firm, false);
                    return new Message.Report(firm);
                }
            }*/
            return null;
        }

        /// <summary>
        /// Преобразование сообщения в класс MarketClass
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private MsgReport? GetClassFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SClass));
            var sclass = json.ReadObject(ms) as SClass;
            ms.Close();

            MarketClass marketClass = new MarketClass();
            marketClass.FirmId = sclass.firmid;
            marketClass.Firm = Trader.tFirms.SearchFirst(f => f.Id == marketClass.FirmId);
            marketClass.Code = sclass.code;
            marketClass.Name = sclass.name;

            marketClass.CountParams = sclass.npars.ToInt32();
            marketClass.CountSecurities = sclass.nsecs.ToInt32();

            if (marketClass.Code != null)
            {
                Trader.tClasses.Add(marketClass, false);
                return new MsgReport() { Object = marketClass };
            }

            /*if (msg.Struct.Length > 0)
            {
                //System.IO.File.AppendAllText(@"debug_file_111.txt", msg.Message + "\n");
                MarketClass marketClass = new MarketClass();
                //0 OnClass | 1 firmid | 2 code | 3 name | 4 npars | 5 nsecs
                if (MarketClasses.Count > 0)
                {
                    var findClass = MarketClasses.FirstOrDefault(e => e == msg.Struct[2]);
                    if (findClass.Empty()) return null;
                }

                marketClass.FirmId = msg.Struct[1];
                marketClass.Firm = Trader.tFirms.SearchFirst(f => f.Id == marketClass.FirmId);
                marketClass.Code = msg.Struct[2];
                marketClass.Name = msg.Struct[3];

                marketClass.CountParams = msg.Struct[4].ToInt32();
                marketClass.CountSecurities = msg.Struct[5].ToInt32();

                if (marketClass.Code != null)
                {
                    Trader.tClasses.Add(marketClass, false);
                    return new Message.Report(marketClass);
                }
            }*/
            return null;
        }

        /*
        /// <summary>
        /// Преобразование сообщения в класс Securities
        /// </summary>
        /// <param name="m">Сообщение</param>
        /// <param name="Msg">Разбитое сообщение на массив</param>
        /// <param name="Trader">Объект Trader</param>
        /// <returns></returns>
        public Securities GetSecuritiesFromArrayMsg(Message msg)
		{
			try
			{
				if (msg.Struct.Length > 0)
				{
					Securities NewSec = new Securities();
					// 0 OnSecurities | 1 sec_code | 2 class_code | 3 name | 4 short_name | 5 face_unit | 
					// 6 scale | 7 face_value | 8 lot_size | 9 mat_date | 10 min_step_price

					NewSec.Code = msg.Struct[1];
					NewSec.ClassCode = msg.Struct[2];
					//Проверка наличия класса в списке разрешенных
					if (!ServiceConvertorMsg.CodesClassForStock.Contains(NewSec.ClassCode))
						return null;
					NewSec.Class = Trader.Classes.ToArray().FirstOrDefault(c => c.Code == NewSec.ClassCode);
					Trader.tSecurities.LockCollection();
					var sec = Trader.Securities.FirstOrDefault(s => s.Code == NewSec.Code && s.ClassCode == NewSec.Class.Code);
					Trader.tSecurities.UnLockCollection();
					if (sec.IsNull())
					{
						try { NewSec.Scale = msg.Struct[6].ToInt32(); }
						catch (Exception) { NewSec.Scale = 0; }

						try { NewSec.Params.MinPriceStep = msg.Struct[10].ToDecimal(); }
						catch (Exception) { NewSec.Params.MinPriceStep = 0; }

						NewSec.Lot = msg.Struct[8].ToInt32();

						NewSec.Name = msg.Struct[3];
						NewSec.Shortname = msg.Struct[4];
						NewSec.Params.FaceUnit = msg.Struct[5];

						try { NewSec.Params.FaceValue = msg.Struct[7].ToDecimal(); }
						catch (Exception) { NewSec.Params.FaceValue = 0; }

						NewSec.Params.MatDate = msg.Struct[9] != "" ? msg.Struct[9].ToLong() : 0;
					}
					else
					{
						NewSec = null;
					}

					if (NewSec.NotIsNull())
					{
						NewSec.Status = 0;
						Trader.tSecurities.MaxElementInList = 1000;
						Trader.tSecurities.Add(NewSec, false);
						NewSec.Trades = Trader.Trades.Where(o => o.Sec == NewSec);
						NewSec.Orders = Trader.Orders.Where(so => so.Sec == NewSec);
						NewSec.StopOrders = Trader.StopOrders.Where(so => so.Sec == NewSec);
						NewSec.MyTrades = Trader.MyTrades.Where(t => t.Order.Sec == NewSec);
						return NewSec;
					}
				}
			}
			catch (Exception e)
			{
				Qlog.Write(e.ToString() + msg.Message);
			}
			return null;
		}*/

        private MsgReport? GetSecuritiesFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SSecurities));
            var ssec = json.ReadObject(ms) as SSecurities;
            ms.Close();

            var Class = Trader.tClasses.SearchFirst(c => c.Code == ssec.class_code);
            Securities sec = new Securities
            {
                Class = Class,
                Code = ssec.code,
                Scale = ssec.scale.ToInt32(),
                Name = ssec.name,
                Shortname = ssec.short_name,
                Lot = ssec.lot_size.ToInt32(),
            };
            sec.Params.MinPriceStep = ssec.min_price_step.ToDecimalE(sec.Scale);


            Trader.tSecurities.Add(sec, false);
            return new MsgReport() { Object = sec };
        }


        /// <summary>
        /// Преобразование сообщения в класс ChangeSecurities
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        private MsgReport? GetChangeSecuritiesFromArrayMsg(Msg msg, bool newItem = true)
        {
            // 0 OnChangeSecurities| 1 CODE | 2 0.000000 | 3 ROSN | 4 CLASS_CODE | 5 0.000000 | 6 QJSIM | 7 SEC_SCALE | 8 2.000000 | 9 2 |
            // STATUS | 0.000000 |  | LOTSIZE | 10.000000 | 10 | BID | 311.400000 | 311,40 | 
            // BIDDEPTH | 0.000000 |  | BIDDEPTHT | 1260.000000 | 1 260 | NUMBIDS | 0.000000 |  | 
            // OFFER | 311.500000 | 311,50 | OFFERDEPTH | 0.000000 |  | OFFERDEPTHT | 1480.000000 | 1 480 | 
            // NUMOFFERS | 0.000000 |  | OPEN | 316.000000 | 316,00 | HIGH | 0.000000 |  | LOW | 0.000000 |  | 
            // LAST | 311.500000 | 311,50 | CHANGE | 0.000000 |  | QTY | 0.000000 |  | TIME | 171823.000000 | 17:18:23 | 
            // VOLTODAY | 0.000000 |  | VALTODAY | 1378179001.000000 | 1 378 179 001 | 
            // TRADINGSTATUS | 1.000000 | открыта | VALUE | 0.000000 |  | WAPRICE | 0.000000 |  | 
            // HIGHBID | 0.000000 |  | LOWOFFER | 0.000000 |  | NUMTRADES | 33454.000000 | 33 454 | 
            // PREVPRICE | 239.880000 | 239,88 | PREVWAPRICE | 0.000000 |  | CLOSEPRICE | 0.000000 |  | 
            // LASTCHANGE | 0.000000 | 0,00 | PRIMARYDIST | 0.000000 |  | ACCRUEDINT | 0.000000 |  | 
            // YIELD | 0.000000 |  | LONGNAME | 0.000000 | ОАО \"НК \"Роснефть\" | SHORTNAME | 0.000000 | Роснефть | 
            // TRADE_DATE_CODE | 20170528.000000 | 28.05.2017 | MAT_DATE | 0.000000 |  | DAYS_TO_MAT_DATE | 0.000000 |  | 
            // SEC_FACE_VALUE | 1.000000 | 1,00 | SEC_FACE_UNIT | 0.000000 | SUR | 
            // SEC_PRICE_STEP | 0.050000 | 0,05 | SECTYPE | 0.000000 | Ценные бумаги | PRICEMAX | 0.000000 |  | 
            // PRICEMIN | 0.000000 |  | NUMCONTRACTS | 0.000000 |  | BUYDEPO | 0.000000 |  | SELLDEPO | 0.000000 |  | 
            // CHANGETIME | 0.000000 |  | TRADECHANGE | 0.000000 |  | FACEVALUE | 0.000000 |  | 
            // MARKETPRICETODAY | 0.000000 |  | BUYBACKPRICE | 0.000000 |  | BUYBACKDATE | 0.000000 |  | 
            // ISSUESIZE | 0.000000 |  | PREVDATE | 0.000000 |  | LOPENPRICE | 0.000000 |  | 
            // LCURRENTPRICE | 0.000000 |  | LCLOSEPRICE | 0.000000 |  | ISPERCENT | 0.000000 |  | 
            // CLSTATE | 0.000000 |  | CLPRICE | 0.000000 |  | STARTTIME | 0.000000 |  | ENDTIME | 0.000000 |  | 
            // EVNSTARTTIME | 0.000000 |  | EVNENDTIME | 0.000000 |  | CURSTEPPRICE | 0.000000 |  | 
            // REALVMPRICE | 0.000000 |  | MARG | 0.000000 |  | EXPDATE | 0.000000
            /*         if (msg.Content.Length <= 2)
                     {
                         return null;
                     }
                     var sec_code = "";
                     var class_code = "";
                     int Scale = -1;
                     int step = 2;
                     for (int i = 1; i < msg.Content.Length; i++)
                     {
                         if (sec_code.Length == 0 && (msg.Content[i] == "CODE" || msg.Content[i] == "sec_code" || msg.Content[i] == "code"))
                         {
                             sec_code = msg.Content[i + 2];
                             i = i + step;
                             continue;
                         }
                         else if (class_code.Length == 0 && (msg.Content[i] == "CLASS_CODE" || msg.Content[i] == "class_code"))
                         {
                             class_code = msg.Content[i + 2];
                             i = i + step;
                             continue;
                         }
                         else if (Scale < 0 && (msg.Content[i] == "SEC_SCALE" || msg.Content[i] == "scale"))
                         {
                             Scale = msg.Content[i + 2].ToInt32();
                             i = i + step;
                             continue;
                         }
                         if (sec_code.Length > 0 && class_code.Length > 0 && Scale >= 0)
                         {
                             break;
                         }
                     }
                     if (sec_code.Length == 0 || class_code.Length == 0 || Scale < 0)
                     {
                         return null;
                     }
                     var Class = FindClass(class_code);

                     Securities findSec = null;
                     if (!newItem)
                     {
                         findSec = FindSecurities(sec_code, Class.Code);
                     }
                     if (findSec.IsNull())
                     {
                         findSec = new Securities
                         {
                             Class = Class,
                             Code = sec_code,
                             Scale = Scale
                         };
                         newItem = true;
                     }

                     for (int i = 1; i < msg.Content.Length; i++)
                     {
                         if (msg.Content[i] == "STATUS")
                         {
                             findSec.Params.Status = msg.Content[i + 1].ToInt32();
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "SEC_FACE_UNIT" || msg.Content[i] == "face_unit")
                         {
                             findSec.Params.FaceUnit = msg.Content[i + 2];
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "STEPPRICE")
                         {
                             findSec.Params.StepPrice = msg.Content[i + 1].ToDecimal(2);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "PRICEMIN")
                         {
                             findSec.Params.MinPrice = msg.Content[i + 1].ToDecimal(2);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "PRICEMAX")
                         {
                             findSec.Params.MaxPrice = msg.Content[i + 1].ToDecimal(2);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "BID")
                         {
                             findSec.Params.Bid = msg.Content[i + 1].ToDecimal(findSec.Scale);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "OFFER")
                         {
                             findSec.Params.Ask = msg.Content[i + 1].ToDecimal(findSec.Scale);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "BIDDEPTH")
                         {
                             findSec.Params.BidDepth = msg.Content[i + 1].ToDecimal(findSec.Scale);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "OFFERDEPTH")
                         {
                             findSec.Params.AskDepth = msg.Content[i + 1].ToDecimal(findSec.Scale);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "BIDDEPTHT")
                         {
                             findSec.Params.SumBidDepth = msg.Content[i + 1].ToDecimal(0);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "OFFERDEPTHT")
                         {
                             findSec.Params.SumAskDepth = msg.Content[i + 1].ToDecimal(0);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "NUMBIDS")
                         {
                             findSec.Params.NumBid = msg.Content[i + 1].ToLong();
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "NUMOFFERS")
                         {
                             findSec.Params.NumAsk = msg.Content[i + 1].ToLong();
                             i = i + step;
                             continue;
                         }
                         if (msg.Content[i] == "BUYDEPO")
                         {
                             findSec.Params.BuyDepo = msg.Content[i + 1].ToDecimal(2);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "SELLDEPO")
                         {
                             findSec.Params.SellDepo = msg.Content[i + 1].ToDecimal(2);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "SEC_FACE_VALUE" || msg.Content[i] == "face_value")
                         {
                             try
                             {
                                 findSec.Params.FaceValue = msg.Content[i + 1].ToDecimal();
                             }
                             catch (Exception) { }
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "SHORTNAME" || msg.Content[i] == "short_name")
                         {
                             findSec.Shortname = msg.Content[i + 2];
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "LONGNAME" || msg.Content[i] == "name")
                         {
                             findSec.Name = msg.Content[i + 2];
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "LOTSIZE" || msg.Content[i] == "lot_size")
                         {
                             findSec.Lot = msg.Content[i + 1].ToInt32();
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "SEC_PRICE_STEP" || msg.Content[i] == "min_price_step")
                         {
                             try
                             {
                                 findSec.Params.MinPriceStep = msg.Content[i + 1].ToDecimal(findSec.Scale);
                             }
                             catch (Exception)
                             {
                                 findSec.Params.MinPriceStep = 0;
                             }
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "MAT_DATE" || msg.Content[i] == "mat_date")
                         {
                             findSec.Params.MatDate = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToLong() : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "SECTYPE")
                         {
                             findSec.Params.SecType = msg.Content[i + 2];
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "DAYS_TO_MAT_DATE")
                         {
                             findSec.Params.DaysToMatDate = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToInt32() : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "TIME")
                         {
                             if (msg.Content[i + 2] != "") findSec.Params.TimeLastTrade = TimeSpan.Parse(msg.Content[i + 2]);
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "QTY")
                         {
                             findSec.Params.VolumeLastTrade = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToInt32() : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "LAST")
                         {
                             findSec.Params.LastPrice = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToDecimal(findSec.Scale) : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "OPEN")
                         {
                             findSec.Params.Open = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToDecimal(findSec.Scale) : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "HIGH")
                         {
                             findSec.Params.High = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToDecimal(findSec.Scale) : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "LOW")
                         {
                             findSec.Params.Low = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToDecimal(findSec.Scale) : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "VALTODAY")
                         {
                             findSec.Params.ValToday = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToDecimal(2) : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "TRADINGSTATUS")
                         {
                             findSec.Params.TradingStatus = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToInt32() : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "NUMTRADES")
                         {
                             findSec.Params.NumTrades = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToLong() : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "PREVPRICE")
                         {
                             findSec.Params.PriceClose = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToDecimal(findSec.Scale) : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "TRADE_DATE_CODE")
                         {
                             if (msg.Content[i + 2] != "") findSec.Params.TradeDate = DateMarket.ExtractDateTime(Convert.ToDateTime(msg.Content[i + 2]));
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "YIELD")
                         {
                             findSec.Params.Yield = msg.Content[i + 1] != "" ? msg.Content[i + 1].ToDecimal(2) : 0;
                             i = i + step;
                             continue;
                         }
                         else if (msg.Content[i] == "isin_code")
                         {
                             findSec.Params.ISIN = msg.Content[i + 2];
                             i = i + step;
                             continue;
                         }
                     }
                     if (findSec.NotIsNull())
                     {
                         if (newItem)
                         {
                             Trader.tSecurities.Add(findSec, false);
                         }
                         return new MsgReport(findSec);
                     }*/
            return null;
        }

        /// <summary>
        /// Преобразование сообщения в класс Account
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private MsgReport? GetAccountsFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SAccount));
            var saccount = json.ReadObject(ms) as SAccount;
            ms.Close();

            Account acc = new Account();
            string[] listC = saccount.class_codes.Trim('|').Split('|');
            listC.ToList().ForEach(el =>
            {
                if (el.NotIsNull() && el.Length > 0)
                {
                    var cl = Trader.tClasses.SearchFirst(c => c.Code == el);
                    if (cl.NotIsNull())
                    {
                        acc.AccClasses.Add(cl);
                    }
                    else
                    {
                        cl = null;
                    }
                }
            });

            acc.Firm = Trader.tFirms.SearchFirst(f => f.Id == saccount.firmid);
            if (acc.Firm.IsNull())
            {
                var newFirm = new Firm()
                {
                    Id = saccount.firmid,
                    Name = saccount.firmid,
                    Status = 1,
                    Exchange = saccount.firmid
                };
                Trader.tFirms.Add(newFirm);
                acc.Firm = newFirm;
            }
            acc.AccID = saccount.trdaccid;
            acc.AccType = saccount.trdacc_type.ToInt32();

            if (acc.AccID.NotIsNull() && acc.AccID.Length > 0)
            {
                Trader.tAccounts.Add(acc);
                return new MsgReport() { Object = acc };
            }

            /*
            if (msg.Struct.Length > 0)
            {
                Account acc = new Account();
                // 0 OnAccount | 1 class_codes | 2 firmid | 3 trdaccid | 4 trdacc_type
                string[] listC = msg.Struct[1].Trim('|').Split('|');
                listC.ToList().ForEach(el =>
                {
                    if (el != "" && el.NotIsNull())
                    {
                        var cl = Trader.tClasses.SearchFirst(c => c.Code == el);
                        if (cl.NotIsNull()) acc.AccClasses.Add(cl);
                        else cl = null;
                    }
                });
                acc.Firm = Trader.tFirms.SearchFirst(f => f.Id == msg.Struct[2]);
                if (acc.Firm.IsNull())
                {
                    var newFirm = new Firm()
                    {
                        Id = msg.Struct[2],
                        Name = msg.Struct[2],
                        Status = 1,
                        Exchange = msg.Struct[2]
                    };
                    Trader.tFirms.Add(newFirm);
                    acc.Firm = newFirm;
                }
                acc.AccID = msg.Struct[3];
                acc.AccType = msg.Struct[4].ToInt32();

                if (acc.AccID.NotIsNull() && acc.AccID != "")
                {
                    Trader.tAccounts.Add(acc);
                    return new Message.Report(acc);
                }
            }*/
            return null;
        }

        /// <summary> Получение объекта "Клиент" из сообщения. </summary>
        /// <param name="msg"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private MsgReport? GetClientsFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SClient));
            var sclient = json.ReadObject(ms) as SClient;
            ms.Close();

            Client cl = new Client();
            if (sclient.client_codes.Length > 0)
            {
                //Для избежания дублирования
                int tmp = (int)(sclient.client_codes.Length / 2);
                var code1 = sclient.client_codes.Substring(0, tmp);
                var code2 = sclient.client_codes.Substring(tmp, tmp);
                cl.Code = code1 == code2 ? code1 : sclient.client_codes;
                if (cl.Code.Length > 0)
                {
                    Trader.tClients.Add(cl, false);
                    return new MsgReport() { Object = cl };
                }
            }


            return null;
        }

        /// <summary> Получение аккаунта из сообщения </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        private MsgReport? GetAccountPositionFromArrayMsg(Msg msg)
        {
            return null;
        }
        private MsgReport? GetDepoLimitFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SDepoLimit));
            var sdepoLimit = json.ReadObject(ms) as SDepoLimit;
            ms.Close();

            long limit_kind = sdepoLimit.limit_kind.ToInt64();
            if (limit_kind < 0)
            {
                return null;
            }

            Position position = null;
            // 0 OnDepoLimit | 1 limit_kind | 2 sec_code | 3 trdaccid | 4 firmid | 5 client_code | 
            // 6 openbal | 7 openlimit | 8 currentbal | 9 currentlimit | 10 locked_sell | 
            // 11 locked_buy | 12 locked_buy_value | 13 locked_sell_value | 14 awg_position_price
            string SecCode = sdepoLimit.sec_code;
            Securities Sec = FindSecurities(SecCode);
            if (Sec.IsNull())
            {
                return null;
            }
            Firm firm = this.FindFirm(sdepoLimit.firmid);
            Account Account = Trader.tAccounts.SearchFirst(a => a.AccID == sdepoLimit.trdaccid);
            Client client = Trader.tClients.SearchFirst(c => c.Code.Contains(sdepoLimit.client_code));

            if (Account.NotIsNull() && firm.NotIsNull())
            {
                position = Trader.tPositions.SearchFirst(
                    p => p.Sec == Sec
                    && p.Account == Account
                    && p.Client == client
                    && p.Data.Type == limit_kind);
            }
            bool change = false;
            if (position.IsNull())
            {
                position = new Position
                {
                    SecCode = SecCode,
                    Account = Account,
                    Firm = firm,
                    Client = client,
                    Sec = Sec,
                };
                position.Data.Type = (short)limit_kind;
                Trader.tPositions.Add(position, false);
            }
            else change = true;

            position.Data.StartNet = position.Data.StartBuy = GetRealVolume(Sec, sdepoLimit.openbal.ToInt32());
            position.Data.OpenLimit = sdepoLimit.openlimit.ToDecimal(0);

            position.Data.CurrentNet = position.Data.TodayBuy = GetRealVolume(Sec, sdepoLimit.currentbal.ToInt32());

            position.Data.PositionValue = sdepoLimit.currentlimit.ToDecimal(2);

            position.Data.OrdersSell = GetRealVolume(Sec, sdepoLimit.locked_sell.ToInt32());
            position.Data.OrdersBuy = GetRealVolume(Sec, sdepoLimit.locked_buy.ToInt32());

            Qlog.CatchException(() =>
            {
                position.Data.LockedBuyValue = sdepoLimit.locked_buy_value.ToDecimal(2);
                position.Data.LockedSellValue = sdepoLimit.locked_sell_value.ToDecimal(2);
                position.Data.AwgPositionPrice = sdepoLimit.awg_position_price.ToDecimal(2);
            });

            if (change)
            {
                Trader.tPositions.Change(position, false);
            }
            return new MsgReport() { Object = position };
            /*
            if (msg.Struct.Length > 0)
            {
                long limit_kind = msg.Struct[1].ToInt64();
                if (limit_kind < 0)
                {
                    return null;
                }
                Position position = null;
                // 0 OnDepoLimit | 1 limit_kind | 2 sec_code | 3 trdaccid | 4 firmid | 5 client_code | 
                // 6 openbal | 7 openlimit | 8 currentbal | 9 currentlimit | 10 locked_sell | 
                // 11 locked_buy | 12 locked_buy_value | 13 locked_sell_value | 14 awg_position_price
                string SecCode = msg.Struct[2];
                Securities Sec = FindSecurities(SecCode);
                if (Sec.IsNull())
                {
                    return null;
                }
                Firm firm = this.FindFirm(msg.Struct[4]);
                Account Account = Trader.tAccounts.SearchFirst(a => a.AccID == msg.Struct[3]);
                Client client = Trader.tClients.SearchFirst(c => c.Code.Contains(msg.Struct[5]));

                if (Account.NotIsNull() && firm.NotIsNull())
                {
                    position = Trader.tPositions.SearchFirst(
                        p => p.Sec == Sec
                        && p.Account == Account
                        && p.Client == client
                        && p.Data.Type == limit_kind);
                }
                bool change = false;
                if (position.IsNull())
                {
                    position = new Position
                    {
                        SecCode = SecCode,
                        Account = Account,
                        Firm = firm,
                        Client = client,
                        Sec = Sec,
                    };
                    position.Data.Type = (short)limit_kind;
                    Trader.tPositions.Add(position, false);
                }
                else change = true;

                position.Data.StartNet = position.Data.StartBuy = GetRealVolume(Sec, msg.Struct[6].ToInt32());
                position.Data.OpenLimit = msg.Struct[7].ToDecimal(0);

                position.Data.CurrentNet = position.Data.TodayBuy = GetRealVolume(Sec, msg.Struct[8].ToInt32());

                position.Data.PositionValue = msg.Struct[9].ToDecimal(2);

                position.Data.OrdersSell = GetRealVolume(Sec, msg.Struct[10].ToInt32());
                position.Data.OrdersBuy = GetRealVolume(Sec, msg.Struct[11].ToInt32());

                Qlog.CatchException(() =>
                {
                    position.Data.LockedBuyValue = msg.Struct[12].ToDecimal(2);
                    position.Data.LockedSellValue = msg.Struct[13].ToDecimal(2);
                    position.Data.AwgPositionPrice = msg.Struct[14].ToDecimal(2);
                });

                if (change)
                {
                    Trader.tPositions.Change(position, false);
                }
                return new Message.Report(position);
            }*/
        }


        /// <summary>
        /// Получает реальный обьем по инструменту
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        private int GetRealVolume(Securities sec, int volume)
        {
            if (sec.IsNull()) return 0;
            return sec.StepPrice > 0
                ? volume
                : (sec.Lot != 0 ? volume / sec.Lot : 0);
        }

        /// <summary>
        /// Получение лимитов по фьючерсам из сообщения. (Позиции)
        /// </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        private MsgReport? GetFuturesHoldingFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SFuturesHolding));
            var sfutHolding = json.ReadObject(ms) as SFuturesHolding;
            ms.Close();

            short type = sfutHolding.type.ToInt16();
            //if (type != 0) return null;

            Position position = null;
            Securities Sec = this.FindSecurities(sfutHolding.sec_code);
            if (Sec.IsNull())
            {
                return null;
            }
            Firm firm = this.FindFirm(sfutHolding.firmid);
            Account Account = Trader.tAccounts.SearchFirst(a => a.AccID == sfutHolding.trdaccid);
            Client client = Trader.tClients.SearchFirst(c => c.Code.Contains(sfutHolding.trdaccid));

            if (sfutHolding.sec_code != "" && Account != null && firm != null)
            {
                position = Trader.tPositions.SearchFirst(p =>
                p.SecCode == sfutHolding.sec_code
                && p.Account.AccID == Account.AccID);
            }
            bool change = false;
            if (position == null)
            {
                position = new Position
                {
                    SecCode = sfutHolding.sec_code,
                    Account = Account,
                    Firm = firm,
                    Sec = Sec,
                    Client = client
                };
                position.Data.Type = type;
                Trader.tPositions.Add(position, false);
            }
            else change = true;

            position.Data.StartBuy = sfutHolding.startbuy.ToInt32();
            position.Data.StartSell = sfutHolding.startsell.ToInt32();
            position.Data.StartNet = sfutHolding.startnet.ToInt32();

            position.Data.OrdersBuy = sfutHolding.openbuys.ToInt32();
            position.Data.OrdersSell = sfutHolding.opensells.ToInt32();
            position.Data.TodayBuy = sfutHolding.todaybuy.ToInt32();
            position.Data.TodaySell = sfutHolding.todaysell.ToInt32();
            position.Data.CurrentNet = sfutHolding.totalnet.ToInt32();

            position.Data.TotalVarMargin = sfutHolding.total_varmargin.ToDecimal(2);
            position.Data.RealVarMargin = sfutHolding.real_varmargin.ToDecimal(2);
            position.Data.AwgPositionPrice = sfutHolding.avrposnprice.ToDecimal(2);
            position.Data.PositionValue = sfutHolding.positionvalue.ToDecimal(2);
            position.Data.SessionStatus = sfutHolding.session_status.ToDecimal(2);
            position.Data.VarMargin = sfutHolding.varmargin.ToDecimal(2);
            position.Data.CbplPlanned = sfutHolding.cbplplanned.ToDecimal(2);
            position.Data.CbplUsed = sfutHolding.cbplused.ToDecimal(2);

            //Обрабатываем сообщения с типом 0
            if (change)
            {
                Trader.tPositions.Change(position, false);
            }
            return new MsgReport() { Object = position };

            /*
            if (msg.Struct.Length > 0)
            {
                short type = msg.Struct[13].ToInt16();
                //if (type != 0) return null;

                Position position = null;
                // 0 OnFuturesHolding | 1 sec_code | 2 firmid | 3 trdaccid | 4 startbuy | 5 startnet | 6 startsell | 
                // 7 openbuys | 8 opensells | 9 todaybuy | 10 totalnet | 11 todaysell,
                // 12 real_varmargin | 13 type | 14 avrposnprice | 15 positionvalue | 16 total_varmargin |
                // 17 session_status | 18 varmargin | 19 cbplplanned | 20 cbplused |

                string SecCode = msg.Struct[1];
                Securities Sec = this.FindSecurities(SecCode);
                if (Sec.IsNull()) return null;
                Firm firm = this.FindFirm(msg.Struct[2]);
                Account Account = Trader.tAccounts.SearchFirst(a => a.AccID == msg.Struct[3]);
                Client client = Trader.tClients.SearchFirst(c => c.Code.Contains(msg.Struct[3]));

                if (SecCode != "" && Account != null && firm != null)
                {
                    position = Trader.tPositions.SearchFirst(p =>
                    p.SecCode == SecCode
                    && p.Account.AccID == Account.AccID);
                }
                bool change = false;
                if (position == null)
                {
                    position = new Position
                    {
                        SecCode = SecCode,
                        Account = Account,
                        Firm = firm,
                        Sec = Sec,
                        Client = client
                    };
                    position.Data.Type = type;
                    Trader.tPositions.Add(position, false);
                }
                else change = true;

                position.Data.StartBuy = msg.Struct[4].ToInt32();
                position.Data.StartSell = msg.Struct[6].ToInt32();
                position.Data.StartNet = msg.Struct[5].ToInt32();

                position.Data.OrdersBuy = msg.Struct[7].ToInt32();
                position.Data.OrdersSell = msg.Struct[8].ToInt32();
                position.Data.TodayBuy = msg.Struct[9].ToInt32();
                position.Data.TodaySell = msg.Struct[11].ToInt32();
                position.Data.CurrentNet = msg.Struct[10].ToInt32();

                position.Data.TotalVarMargin = msg.Struct[16].ToDecimal(2);
                position.Data.RealVarMargin = msg.Struct[12].ToDecimal(2);
                position.Data.AwgPositionPrice = msg.Struct[14].ToDecimal(2);
                position.Data.PositionValue = msg.Struct[15].ToDecimal(2);
                position.Data.SessionStatus = msg.Struct[17].ToDecimal(2);
                position.Data.VarMargin = msg.Struct[18].ToDecimal(2);
                position.Data.CbplPlanned = msg.Struct[19].ToDecimal(2);
                position.Data.CbplUsed = msg.Struct[20].ToDecimal(2);

                //Обрабатываем сообщения с типом 0
                if (change)
                {
                    Trader.tPositions.Change(position, false);
                }
                return new Message.Report(position);
            }
            return null;*/
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private MsgReport? GetPortfoliosFromArrayMsg(Msg msg)
        {
            Portfolio portfolio = null;
            // 0 OnPortfolioInfo | 1 firmid | 2 client_code | 3 limit_kind | 4 portfolio_value | 
            // 5 is_marginal | 6 total_money_bal | 7 fut_total_asset | 8 fundslevel | 
            // 9 curr_tag | 10 current_bal | 11 client_type | 12 all_assets | 
            // 13 rate_change | 14 in_assets | 15 in_all_assets | 16 is_leverage | 
            // 17 profit_loss | 18 assets | 19 rate_futures | 20 lim_non_margin
            /*          var Acc = Trader.tAccounts.SearchFirst(a => a.Firm.Id == msg.Content[1]);
                      if (Acc.IsNull()) Acc = Trader.tAccounts.SearchFirst(a => a.Firm.Id == msg.Content[1]);
                      var client = Trader.tClients.SearchFirst(c => c.Code.Contains(msg.Content[2]));
                      var limitKind = msg.Content[3].ToInt32();
                      if (Acc.NotIsNull())
                      {
                          portfolio = Trader.tPortfolios.SearchFirst(p => p.Account.AccID == Acc.AccID
                              && p.Account.Firm.Id == Acc.Firm.Id && p.Client.Code == client.Code && p.LimitKind == limitKind);
                      }
                      else
                      {
                          //Если счет не найдет портвель не добавляем
                          return null;
                      }
                      if (portfolio.IsNull())
                      {
                          portfolio = new Portfolio
                          {
                              Account = Acc,
                              Client = client,
                              LimitKind = limitKind
                          };
                          Trader.tPortfolios.Add(portfolio, false);
                      }

                      portfolio.Balance = msg.Content[12].ToDecimal(2);
                      portfolio.CurrentBalance = msg.Content[20].ToDecimal(2);
                      portfolio.LastPositionBalance = msg.Content[15].ToDecimal(2);
                      portfolio.PositionBalance = portfolio.Balance - portfolio.CurrentBalance;//Math.Round(Convert.ToDecimal(msg.Struct[12].Replace('.', ',')), 2);
                      portfolio.VarMargin = msg.Content[17].ToDecimal(2);
                      portfolio.RateChange = msg.Content[13].ToDecimal();
                      portfolio.TypeClient = msg.Content[11].ToInt32();

                      //porFind.Tag = msg.Struct[13];
                      return new MsgReport() { Object = portfolio };
            */
            return null;
        }

        /// <summary>
        /// Получение портфеля и его изменений из сообщения
        /// </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        private MsgReport? GetMoneyLimitsFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SMoneyLimits));
            var smonetLimits = json.ReadObject(ms) as SMoneyLimits;
            ms.Close();

            Portfolio portfolio = null;
            // 0 OnMoneyLimits | 1 leverage | 2 currentbal | 3 limit_kind | 4 client_code | 5 openlimit | 6 firmid | 
            // 7 locked_margin_value | 8 currcode | 9 openbal | 10 locked | 11 locked_value_coef | 12 currentlimit | 13 tag
            var Acc = Trader.tAccounts.SearchFirst(a => a.Firm.Id == smonetLimits.firmid);
            var client = Trader.tClients.SearchFirst(c => c.Code.Contains(smonetLimits.client_code));
            if (Acc.NotIsNull())
            {
                portfolio = Trader.tPortfolios.SearchFirst(p => p.Account.AccID == Acc.AccID
                    && p.Account.Firm.Id == Acc.Firm.Id
                    && p.Client.Code == client.Code
                    && p.LimitKind == smonetLimits.limit_kind.ToInt32());
            }

            if (portfolio.IsNull())
            {
                portfolio = new Portfolio
                {
                    Account = Acc,
                    Client = client,
                    LimitKind = smonetLimits.limit_kind.ToInt32()
                };

                Trader.tPortfolios.Add(portfolio, false);
            }
            //Ответ на сервер
            var Reply = String.Join(
                MsgServer.SP_FORSERVER.ToString(),
                new string[] { "GetPortfolioInfo", Acc.Firm.Id, client.Code, smonetLimits.limit_kind }
                );

            portfolio.CurrentBalance = smonetLimits.currentbal.ToDecimal();
            //porFind.PositionBalance = Convert.ToDecimal(msg.Struct[2].Replace('.', ','));
            //porFind.VarMargin = Convert.ToDecimal(msg.Struct[5].Replace('.', ','));
            portfolio.Tag = smonetLimits.tag;
            portfolio.LockedBalance = smonetLimits.locked.ToDecimal(2);
            /*
            if (change)

            {
                Trader.tPortfolios.Change(portfolio, false);// !MManager.EventPackages);
            }
            //if (porFind.Account != null)
            */

            return new MsgReport()
            {
                Object = portfolio,
                Reply = Reply
            };
            /*
            if (msg.Struct.Length > 0)
            {
                Portfolio portfolio = null;
                // 0 OnMoneyLimits | 1 leverage | 2 currentbal | 3 limit_kind | 4 client_code | 5 openlimit | 6 firmid | 
                // 7 locked_margin_value | 8 currcode | 9 openbal | 10 locked | 11 locked_value_coef | 12 currentlimit | 13 tag
                var Acc = Trader.tAccounts.SearchFirst(a => a.Firm.Id == msg.Struct[6]);
                var client = Trader.tClients.SearchFirst(c => c.Code.Contains(msg.Struct[4]));
                if (Acc.NotIsNull())
                {
                    portfolio = Trader.tPortfolios.SearchFirst(p => p.Account.AccID == Acc.AccID
                        && p.Account.Firm.Id == Acc.Firm.Id
                        && p.Client.Code == client.Code
                        && p.LimitKind == msg.Struct[3].ToInt32());
                }

                if (portfolio.IsNull())
                {
                    portfolio = new Portfolio
                    {
                        Account = Acc,
                        Client = client,
                        LimitKind = msg.Struct[3].ToInt32()
                    };

                    Trader.tPortfolios.Add(portfolio, false);
                }
                //Ответ на сервер
                var Reply = String.Join(
                    Message.SP_FORSERVER.ToString(),
                    new string[] { "GetPortfolioInfo", Acc.Firm.Id, client.Code, msg.Struct[3] }
                    );


                portfolio.CurrentBalance = Convert.ToDecimal(msg.Struct[2].Replace('.', ','));
                //porFind.PositionBalance = Convert.ToDecimal(msg.Struct[2].Replace('.', ','));
                //porFind.VarMargin = Convert.ToDecimal(msg.Struct[5].Replace('.', ','));
                portfolio.Tag = msg.Struct[13];
                portfolio.LockedBalance = msg.Struct[10].ToDecimal(2);
                /*
                if (change)
                {
                    Trader.tPortfolios.Change(portfolio, false);// !MManager.EventPackages);
                }
                //if (porFind.Account != null)

                return new Message.Report()
                {
                    Object = portfolio,
                    Reply = Reply
                };
            }
            return null;
        */
        }
        /// <summary>
        /// Получение портфеля и его изменений из сообщения
        /// </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        private MsgReport? GetFutLimitsFromArrayMsg(string msg)
        {
            var encode = Encoding.UTF8.GetBytes(msg);
            var ms = new MemoryStream(encode);
            var json = new DataContractJsonSerializer(typeof(SFuturesLimit));
            var sfutLimit = json.ReadObject(ms) as SFuturesLimit;
            ms.Close();

            Portfolio portfolio = null;
            // 0 OnFuturesLimits | 1 cbplused | 2 cbp_prev_limit | 3 varmargin | 4 options_premium | 5 limit_type | 
            // 6 firmid | 7 currcode | 8 cbplused_for_orders | 9 liquidity_coef | 10 real_varmargin | 
            // 11 cbplused_for_positions | 12 accruedint | 13 kgo | 14 ts_comission | 15 cbplplanned | 16 trdaccid | 17 cbplimit
            int LimitKind = sfutLimit.limit_type.ToInt32();
            var Acc = Trader.tAccounts.SearchFirst(a => a.AccID == sfutLimit.trdaccid && a.Firm.Id == sfutLimit.firmid);
            if (Acc.IsNull())
            {
                Acc = Trader.tAccounts.SearchFirst(a => a.Firm.Id == sfutLimit.firmid);
            }
            if (Acc.NotIsNull())
            {
                portfolio = Trader.tPortfolios.SearchFirst(p => p.Account.AccID == Acc.AccID
                    && p.Account.Firm.Id == Acc.Firm.Id && p.LimitKind == LimitKind);
            }

            if (portfolio.IsNull())
            {
                portfolio = new Portfolio
                {
                    Account = Acc,
                    Client = Trader.tClients.SearchFirst(c => c.Code == Acc.AccID)
                };
                Trader.tPortfolios.Add(portfolio, false);
            }

            portfolio.LimitKind = LimitKind;
            portfolio.PrevBalance = sfutLimit.cbp_prev_limit.ToDecimal(2);
            portfolio.CurrentBalance = sfutLimit.cbplplanned.ToDecimal(2);
            portfolio.Balance = sfutLimit.cbplimit.ToDecimal(2);
            portfolio.VarMargin = sfutLimit.varmargin.ToDecimal(2);
            portfolio.PositionBalance = sfutLimit.cbplused.ToDecimal(2);
            portfolio.Commission = sfutLimit.ts_comission.ToDecimal(2);
            portfolio.RealMargin = sfutLimit.real_varmargin.ToDecimal(2);

            return new MsgReport()
            {
                Object = portfolio
                // Reply = Reply
            };

            /*
            if (change)
            {
                Trader.tPortfolios.Change(porFind, false);// !MManager.EventPackages);
            }
            if (porFind.Account != null)
                */
            /*       return new Message.Report()
                   {
                       Object = portfolio
                       // Reply = Reply
                   };

               /*
               if (msg.Struct.Length > 0)
               {
                   Portfolio portfolio = null;
                   // 0 OnFuturesLimits | 1 cbplused | 2 cbp_prev_limit | 3 varmargin | 4 options_premium | 5 limit_type | 
                   // 6 firmid | 7 currcode | 8 cbplused_for_orders | 9 liquidity_coef | 10 real_varmargin | 
                   // 11 cbplused_for_positions | 12 accruedint | 13 kgo | 14 ts_comission | 15 cbplplanned | 16 trdaccid | 17 cbplimit
                   int LimitKind = msg.Struct[5].ToInt32();
                   var Acc = Trader.tAccounts.SearchFirst(a => a.AccID == msg.Struct[16] && a.Firm.Id == msg.Struct[6]);
                   if (Acc.IsNull()) Acc = Trader.tAccounts.SearchFirst(a => a.Firm.Id == msg.Struct[6]);
                   if (Acc.NotIsNull())
                   {
                       portfolio = Trader.tPortfolios.SearchFirst(p => p.Account.AccID == Acc.AccID
                           && p.Account.Firm.Id == Acc.Firm.Id && p.LimitKind == LimitKind);
                   }

                   if (portfolio.IsNull())
                   {
                       portfolio = new Portfolio
                       {
                           Account = Acc,
                           Client = Trader.tClients.SearchFirst(c => c.Code == Acc.AccID)
                       };
                       Trader.tPortfolios.Add(portfolio, false);
                   }

                   portfolio.LimitKind = LimitKind;
                   portfolio.PrevBalance = msg.Struct[2].ToDecimal(2);
                   portfolio.CurrentBalance = msg.Struct[15].ToDecimal(2);
                   portfolio.Balance = msg.Struct[17].ToDecimal(2);
                   portfolio.VarMargin = msg.Struct[3].ToDecimal(2);
                   portfolio.PositionBalance = msg.Struct[1].ToDecimal(2);
                   portfolio.Commission = msg.Struct[14].ToDecimal(2);
                   portfolio.RealMargin = msg.Struct[10].ToDecimal(2);

                   /*
                   if (change)
                   {
                       Trader.tPortfolios.Change(porFind, false);// !MManager.EventPackages);
                   }
                   if (porFind.Account != null)
                       */
            /*       return new Message.Report()
                   {
                       Object = portfolio
                       // Reply = Reply
                   };
               }*/
            return null;
        }

        /// <summary> Получение стакана из сообщения </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        private MsgReport? GetQuoteFromArrayMsg(Msg msg)
        {
            Quote quote = new Quote();
            // 0 OnQuote | 1 QJSIM | 2 MGNT | 3 10.000000 | 4 10.000000 | 
            // 9288.000000 | 16 | 9289.000000 | 17 | 9290.000000 | 5 | 9292.000000 | 11 | 
            // 9295.000000 | 56 | 9300.000000 | 1 | 9301.000000 | 1 | 9306.000000 | 1 | 
            // 9311.000000 | 36 | 9325.000000 | 22 | 9331.000000 | 3 | 9333.000000 | 35 | 
            // 9348.000000 | 57 | 9349.000000 | 6 | 9350.000000 | 20 | 9351.000000 | 50 | 
            // 9352.000000 | 27 | 9353.000000 | 6 | 9356.000000 | 1 | 9366.000000 | 20

            //          var classCode = msg.Content[1];
            /*if (!ServiceConvertorMsg.CodesClassForStock.Contains(classCode))
                return null;*/
            /*          quote.Sec = this.FindSecurities(msg.Content[2], classCode);
                      if (quote.Sec.NotIsNull())
                      {
                          List<Quote.QuoteRow> listBid = new List<Quote.QuoteRow>();
                          List<Quote.QuoteRow> listAsk = new List<Quote.QuoteRow>();
                          int indexStartDepth = 3;
                          bool isAsk = true;
                          for (int i = indexStartDepth; i < msg.Content.Length; i++)
                          {
                              var data = msg.Content[i].Split('=');
                              if (data.Length != 2)
                              {
                                  continue;
                              }
                              if (data[0].Contains("offer"))
                              {
                                  isAsk = true;
                              }
                              else if (data[0].Contains("bid"))
                              {
                                  isAsk = false;
                              }
                              Quote.QuoteRow row = null;
                              if (data[0].Contains("quantity"))
                              {
                                  row = getChartQuote(msg.Content[i], msg.Content[i + 1], quote.Sec);
                                  i = i + 1;
                              }
                              else if (data[0].Contains("price"))
                              {
                                  row = getChartQuote(msg.Content[i + 1], msg.Content[i], quote.Sec);
                                  i = i + 1;
                              }
                              //BID
                              if (!isAsk)
                              {
                                  listBid.Add(row);
                              }
                              //ASK
                              else
                              {
                                  listAsk.Add(row);
                              }
                          }
                          quote.Bid = listBid.ToArray();
                          quote.Ask = listAsk.ToArray();
                          quote.Sec.SetQuote(quote);
                          //Trader.tQuote.Change(Quote, false);
                          return new MsgReport(quote);
                      }*/

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        private Quote.QuoteRow getChartQuote(string volume, string price, Securities sec)
        {
            if (volume.Contains('=') && price.Contains('='))
            {
                var dataVolume = volume.Split('=');
                var dataPrice = price.Split('=');

                Quote.QuoteRow row = new Quote.QuoteRow()
                {
                    Volume = dataVolume[1].ToInt32(),
                    Price = dataPrice[1].ToDecimal(sec.Scale)
                };
                return row;
            }
            return null;
        }
        /// <summary> Обработчик сообщений с транзакциями </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private MsgReport? GetTransReplyFromArrayMsg(Msg msg)
        {
            Reply trans = new Reply();
            // 0 OnTransReply | 1 account | 2 firm_id | 3 order_num, 4 trans_id, 5 price , 
            // 6 quantity, 7 client_code , 8 balance , 9 time , 10 status , 11 exchange_code , 
            // 12 date_time.year | 13 date_time.month | 14 date_time.day | 15 date_time.hour | 16 date_time.min | 
            // 17 date_time.sec | 18 date_time.ms | 19 uid | 
            // 20 result_msg | 21 brokerref | 22 server_trans_id | 23 flags
            /*
                        trans.Account = Trader.tAccounts.SearchFirst(a => a.AccID == msg.Content[1]);
                        trans.Firm = this.FindFirm(msg.Content[2]);

                        //if (trans.Account == null && trans.Firm == null) return null;

                        trans.OrderNumber = msg.Content[3].ToLong();
                        trans.TransID = msg.Content[4].ToLong();
                        trans.Price = msg.Content[5].ToDecimal();
                        trans.Volume = msg.Content[6].ToInt32();
                        trans.Balance = msg.Content[8].ToDecimal();

                        trans.Client = Trader.tClients.SearchFirst(c => c.Code.Contains(msg.Content[7]));
                        trans.Status = msg.Content[10].ToInt32();

                        DateMarket date = new DateMarket();
                        date.SetDateTimeByStruct(12, msg.Content);
                        trans.DateTrans = date.GetDateTime();

                        trans.uid = msg.Content[19].ToDecimal();
                        trans.ResultMsg = msg.Content[20];
                        trans.Comment = msg.Content[21];

                        trans.ServerTransID = msg.Content[22].ToLong();
                        Trader.tTransaction.NewTransReply(trans, true);
                        return new MsgReport(trans);
            */
            return null;
        }
    }
}

