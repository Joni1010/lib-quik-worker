
using MarketObjects;
using System.Threading;
using QuikConnector.Components.Messages;
using QuikConnector.Components.Net;
using QuikConnector.Components.Log;
using QuikConnector.Components.Controllers;

namespace QuikConnector.Components.Terminal
{
    public class TerminalControl : TerminalData
    {
        /// <summary> Флаг подключения к серверной части (к скрипту LUA) </summary>
        private bool connected = false;

        /// <summary> Последнее сообщение полученное от терминала. </summary>
        public string LastMessage = "";
        /// <summary> Последняя полученная сделка </summary>
        public Trade LastTrade = null;

        /// <summary> Настройки для подключения.  </summary>
        private readonly ServerData Server = new ServerData();

        /// <summary> Информация по торговому терминалу  </summary>
        public MarketTerminal Terminal = new MarketTerminal();

        /// <summary> Менеджер сообщений полученных из терминала Quik </summary>
        private readonly MessageController MsgManager = null;

        //Для отладки
        private delegate void AllEvent(string Str);
        //private event AllEvent OnAllEvent;
        /// <summary>  Событие возникновения команд от сервера </summary>
        //private event AllEvent OnAnswerServer;

        //public event Action OnRun;

        /// <summary>
        /// Поток обработки событий 
        /// </summary>
        private Thread thread = null;

        public bool Connected
        {
            get { return connected; }
        }

        /// <summary> Контролер терминала. </summary>
        /// <param name="serverAddr">Адрес подключения к серверу. </param>
        /// <param name="port">Порт подключения</param>
        public TerminalControl(string serverAddr, int portSend, int portReceive)
        {
            MsgManager = new MessageController(this);
            //MsgManTraders = new MManager(this);
            // MsgManMarket = new MManager(this);

            //MsgManager.convertor.onstartmarket += () => {
            //    if (OnRun.NotIsNull())
            //    {
            //        OnRun();
            //    }
            //};

            Server.ServerAddr = serverAddr;
            Server.portSend = portSend;
            Server.portReceive = portReceive;

            thread = ThreadsController.Thread(() =>
            {
                while (thread.NotIsNull())
                {
                    generatingElements();
                    ThreadsController.Sleep(5);
                }
            });
        }

        /// <summary>
        /// Создает сокеты для подключения
        /// </summary>
        public void CreateSockets()
        {
            QLog.CatchException(() =>
            {
                //Обработчик служебных сообщений
                //OnAnswerServer += (command) => { };

                //Конект до сокета сервера
                if (MsgManager.ConnectSockets(Server.ServerAddr, Server.portSend, Server.portReceive))
                {
                    //Инициализация контроллера сообщений
                    MsgManager.InitThreadsMessages();
                    this.connected = true;
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

        /// <summary> Завершает соединение сокета с терминалом</summary>
        public void CloseSockets()
        {
            thread = null;
            this.connected = false;
            MsgManager.Close();
        }
    }
}
