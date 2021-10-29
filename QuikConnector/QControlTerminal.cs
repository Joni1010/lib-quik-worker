using ServiceMessage;

using MarketObjects;
using QLuaApp;
using System.Text.RegularExpressions;
using Connector.Logs;
using System;

namespace QuikControl
{
    public class QControlTerminal : MarketTools
    {
        /// <summary> Флаг подключения к серверной части (к скрипту LUA) </summary>
        public bool isConnected = false;

        /// <summary> Последнее сообщение полученное от терминала. </summary>
        public string LastMessage = "";
        /// <summary> Последняя полученная сделка </summary>
        public Trade LastTrade = null;

        /// <summary> Настройки для подключения.  </summary>
        private QLuaAppServer Server = new QLuaAppServer();
        
        /// <summary> Информация по торговому терминалу  </summary>
        public MarketTerminal Terminal = new MarketTerminal();

        /// <summary> Менеджер сообщений полученных из терминала Quik </summary>
        private MManager MsgManager = null;

        //Для отладки
        private delegate void AllEvent(string Str);
        //private event AllEvent OnAllEvent;
        /// <summary>  Событие возникновения команд от сервера </summary>
        private event AllEvent OnAnswerServer;

        public event Action OnRun;

        /// <summary> Контролер терминала. </summary>
        /// <param name="serverAddr">Адрес подключения к серверу. </param>
        /// <param name="port">Порт подключения</param>
        public QControlTerminal(string serverAddr, int tport, int tportTrade, int tportReceive)
        {
			this.InitMarketTools();

			MsgManager = new MManager(this);
            //MsgManTraders = new MManager(this);
           // MsgManMarket = new MManager(this);

            MsgManager.Convertor.OnStartMarket += () => {
                if (OnRun.NotIsNull())
                {
                    OnRun();
                }
            };

            Server.ServerAddr = serverAddr;
            Server.tPort = tport;
            Server.tPortTrade = tportTrade;
            Server.tPortReceive = tportReceive;
        }

        /// <summary>
        /// Создает сокеты для подключения
        /// </summary>
        public void CreateSockets()
        {
            Qlog.CatchException(() =>
            {
                //Обработчик сообщений от сервера
                MsgManager.OnNewSysMessage += new MManager.eventNewMessage(Event_OnNewSysMessage);
                //Обработчик служебных сообщений
                this.OnAnswerServer += (command) => { };


                //Конект до сокета сервера
                if (MsgManager.ConnectSockets(Server.ServerAddr, Server.tPort, Server.tPortTrade, Server.tPortReceive))
                {
                    //Активатор отложенных событий
                    MsgManager.AcivateAllEvent += () =>
                    {
						this.GenerateAllEvent();
                    };
                    //Инициализация контроллера сообщений
                    MsgManager.InitThreadsMessages();
                    this.isConnected = true;
                }
            });
        }
        /// <summary>
        /// Отправка сообщения на сервер
        /// </summary>
        /// <param name="header">Заголовок сообщения</param>
        /// <param name="msg">Сообщение</param>
        public void SendMsgToServer(string header, string[] msg)
        {
            if (!msg.Empty())
            {
                this.MsgManager.Send(header, msg);
            }
        }

        /// <summary> Событие системного сообщения </summary>
        /// <param name="MsgObject">Объект менеджера сообщений </param>
        /// <param name="message">Строковое сообщение</param>
        private void Event_OnNewSysMessage(MManager MsgObject, string message)
        {
            if (message.Empty()) return;

            Regex reg = new Regex(@"^ServerCommand:", RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(message);
            if (mc.Count > 0)
            {
                string command = message.Replace("ServerCommand:", "");
                if (!OnAnswerServer.Empty())
                {
                    OnAnswerServer(command);
                }
            }
        }

        /// <summary> Завершает соединение сокета с терминалом</summary>
        public void CloseSockets()
        {
            this.isConnected = false;
            MsgManager.OnNewSysMessage -= new MManager.eventNewMessage(Event_OnNewSysMessage);
            MsgManager.Close();
        }
    }
}
