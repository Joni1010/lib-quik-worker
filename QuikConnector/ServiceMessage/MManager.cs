using Connector.Logs;
using Managers;
using QuikControl;
using System.Threading;
using QuikConnector.libs;
using System.Collections.Generic;
using System.Linq;
using System;
using QuikConnector.ServiceMessage.Package;
using QuikConnector.ServiceMessage.Message;
using ServiceMessage;
using System.Text;

namespace QuikConnector.ServiceMessage
{
    /// <summary> Менеджер сообщений </summary>
    public class MManager
    {

        /// <summary> Флаг работы основного цикла. </summary>
        public static bool LoopProcessing = true;
        /// <summary> Размер принимаемого сообщения от сервера. </summary>
        private const int MAX_SIZE_MESSAGE = 10000000;

        /// <summary> Сокет отправки </summary>
        private СSocket SendSocket = new СSocket(MAX_SIZE_MESSAGE);
        /// <summary> Сокет приема базовых сообщений </summary>
        private СSocket GetSocket = new СSocket(MAX_SIZE_MESSAGE);
        /// <summary> Сокет приема сделок </summary>
        private СSocket GetSocketTrades = new СSocket(MAX_SIZE_MESSAGE);

        /// <summary> Стек полученных сообщений </summary>
        private MQueue<Msg> QueueBase = new MQueue<Msg>();
        /// <summary> Стек полученных сообщений с историческими сделками </summary>
        private MQueue<Msg> QueueTrades = new MQueue<Msg>();
        /// <summary> Стек полученных системных сообщений </summary>
        //private MQueue<string> QueueSys = new MQueue<string>();
        /// <summary> Стек сообщений на отправку </summary>
        private MQueue<string> QueueSend = new MQueue<string>();

        /// <summary> Делегат нового сообщения. </summary>
        /// <param name="MsgObject">Объект менеджера сообщений.</param>
        /// <param name="message">Текстовое сообщение.</param>
        public delegate void eventNewMessage(MManager MsgObject, string message);
        /// <summary>  Обработчик нового системного сообщения. </summary>
        public event eventNewMessage OnNewSysMessage;
        /// <summary> Объект конвертора </summary>
        public ConvertorMsg Convertor = null;

        /// <summary> Конструктор объекста Менеджер сообщений </summary>
        /// <param name="trader"></param>
        public MManager(QControlTerminal trader)
        {
            if (!trader.Empty())
            {
                Convertor = new ConvertorMsg(trader);
            }
        }

        /// <summary> Добавить сообщение на отправку серверу  </summary>
        /// <param name="Type">Тип сообщения или заголовок.</param>
        /// <param name="msgSend">Тело сообщения(само сообщение)</param>
        public void Send(string Type, string[] msgSend)
        {
            if (!msgSend.Empty())
            {
                QueueSend.Add(Type + MsgServer.SP_FORSERVER + string.Join(MsgServer.SP_FORSERVER.ToString(), msgSend));
            }
        }
        /// <summary>
        /// Формирует сообщение на сервер
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="msgSend"></param>
        public void Send(string Type, string msgSend)
        {
            if (!msgSend.Empty())
            {
                QueueSend.Add(Type + MsgServer.SP_FORSERVER + msgSend);
            }
        }
        /// <summary> Добавить сообщение на отправку серверу </summary>
        /// <param name="msgSend"></param>
        public void Send(string msgSend)
        {
            if (!msgSend.Empty())
            {
                QueueSend.Add(msgSend);
            }
        }
        /// <summary> Добавить сообщение на отправку серверу с проверкой предыдущей, если совпадает то не добавляет. </summary>
        /// <param name="msgSend"> Отправляемое сообщение </param>
        public void SendCheckLast(string msgSend)
        {
            if (!msgSend.Empty())
            {
                if (msgSend != QueueSend.Last)
                {
                    QueueSend.Add(msgSend);
                }
            }
        }

        /// <summary> Отправка сообщения  </summary>
        private void AllSocketConnected()
        {
            //Запускаем слушатель сокета
            Send("connect_socket", new string[] { "1" });
            Qlog.Write("Socket connect: OK");
        }

        /// <summary> Функция осуществляет подключение к скрипту LUA, запущенный в терминале. </summary>
        /// <param name="ServerAddr">Адрес подключения (по умолчанию localhost)</param>
        /// <param name="port">Порт подключения (по умолчанию 8080)</param>
        /// <returns></returns>
        public bool ConnectSockets(string ServerAddr, int tport, int tportTrade, int tportReceive)
        {
            //Сокет на отправку
            if (!SendSocket.CreateSocket(ServerAddr, tport, false))
            {
                return false;
            }
            //Сокет на получение
            if (!GetSocket.CreateSocket(ServerAddr, tportTrade))
            {
                return false;
            }
            //Сокет на получение исторических сделок
            if (!GetSocketTrades.CreateSocket(ServerAddr, tportReceive))
            {
                return false;
            }
            AllSocketConnected();
            return true;
        }

        /// <summary> Инициализация потоков сообщений, управляет входящими и исходящими сообщениями. </summary>
        public void InitThreadsMessages()
        {
            Thread.Sleep(500);
            ProcessSend();
            //Основной цикл приема сообщений
            MThread.InitThread(ThreadPriority.Normal, this, (classMM) =>
            {
                MManager mm = (MManager)classMM;
                while (LoopProcessing)
                {
                    //Получает данные с сокета
                    mm.Scaning();
                    mm.ScaningHistTrades();
                    Thread.Sleep(1);
                }
                mm.GetSocket.CloseSocket();
            });
            //Основной цикл обработки и отправки сообщений
            MThread.InitThread(ThreadPriority.Normal, this, (classMM) =>
            {
                MManager mm = (MManager)classMM;
                while (LoopProcessing)
                {
                    //Обработка полученных сообщений
                    mm.ProviderMessages();
                    Thread.Sleep(1);
                }
                mm.GetSocket.CloseSocket();
            });
            //Основной цикл обработки и отправки сообщений
            MThread.InitThread(ThreadPriority.Normal, this, (classMM) =>
            {
                MManager mm = (MManager)classMM;
                while (LoopProcessing)
                {
                    mm.ProcessSend();
                    Thread.Sleep(1);
                }
                mm.SendSocket.CloseSocket();
            });
        }

        /// <summary> Функция посылает сообщение на прекращение отправки сообщений сервером, до получения сообщения продолжить. </summary>
        private void StopGettingData()
        {
            SendCheckLast("Stop" + MsgServer.SP_FORSERVER + "1");
        }

        /// <summary>  Обработчик получения данных из сокета  </summary>
        /// <param name="byteRecv">Кол-во байт принятых.</param>
        /// <param name="recvData">Принятые данные</param>

        /// <summary>
        ///  Распределение полученных сообщений из сокета
        /// </summary>
        /// <param name="content">Принятые данные</param>
        /// <param name="isHistoryTrades"> флаг что это историческая сделка </param>
        /// <returns></returns>
        private int Allocation(byte[] data, bool isHistoryTrades = false)
        {
            if (data.Length > 0)
            {
                var content = Encoding.GetEncoding(1251).GetString(Zipper.Unzip(data));
                var pack = PackMsg.Create(content);
                pack.Each((msg) =>
                {
                    if (!isHistoryTrades)
                    {
                        QueueBase.Add(msg);
                    }
                    else
                    {
                        QueueTrades.Add(msg);
                    }
                });
            }
            return data.Length;
        }

        public delegate void ActivatorEvents();
        /// <summary> Событие которое позволяет прогрузить события в очереди. Чтоб избежать застоя. </summary>
        public event ActivatorEvents AcivateAllEvent;

        /// <summary> Обработчик сообщений из стека </summary>
        /// <param name="classMM"></param>
        private bool ProviderMessages()
        {
            while (true)
            {
                //обработка всех важных сообщений
                var message = QueueBase.getFirst;
                QueueBase.DeleteItem(message);
                if (HandlerMessage(message))
                {
                    continue;
                }
                Qlog.Write("Not processed: " + message);
                //throw new ArgumentException("More action " + string.Join(Message.SP_DATA.ToString(), message.Struct));
                //обработка сообщейни исторических сделок
                message = QueueTrades.getFirst;
                QueueTrades.DeleteItem(message);
                if (HandlerMessage(message))
                {
                    continue;
                }
                Qlog.Write("Not processed: " + message);
                //throw new ArgumentException("More action " + string.Join(Message.SP_DATA.ToString(), message.Struct));
                break;
            }
            AcivateAllEvent();
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool HandlerMessage(Msg message)
        {
            if (message.NotIsNull())
            {
                var report = Convertor.NewMessage(message);
                if (report.NotIsNull())
                {
                    //Проверяем надо ли отправить ответ на сервер
                    if (report.Value.Reply.NotIsNull() && report.Value.Reply.Length > 0)
                    {
                        SendCheckLast(report.Value.Reply);
                    }
                    //Проверяем успешность обработки сообщения
                    if (report.Value.Object.NotIsNull())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary> Функция обработки системных сообщений </summary>
        /// <param name="contentMsg">Поступающее сообщение</param>
        /*private void ProcessSysMessage()
        {
            if (QueueSys.Count > 0)
            {
                var item = QueueSys.getFirst;
                QueueSys.DeleteItem(item);
                if (OnNewSysMessage.NotIsNull())
                {
                    OnNewSysMessage(this, item);
                }
            }
        }*/
        /// <summary> Обработка полученных данных </summary>
        private bool Scaning()
        {
            if (GetSocket.Receive() > 0)
            {
                Allocation(GetSocket.StateSock.buffer);
                return true;
            }
            return false;
        }
        /// <summary> Обработка полученных сделок </summary>
        private bool ScaningHistTrades()
        {
            if (GetSocketTrades.Receive() > 0)
            {
                Allocation(GetSocketTrades.StateSock.buffer, true);
                return true;
            }
            return false;
        }

        /// <summary> Функция обработки отправки сообщений </summary>
        /// <param name="contentMsg">Поступающее сообщение</param>
        private void ProcessSend()
        {
            if (QueueSend.Count > 0)
            {
                var item = QueueSend.getFirst;
                QueueSend.DeleteItem(item);
                var bytes = SendSocket.Send(item + '\t');
            }
        }

        /// <summary> Закрыть соединение и прекратить передачу сообщений</summary>
        public void Close()
        {
            StopGettingData();
            LoopProcessing = false;

            SendSocket.CloseSocket();
            GetSocketTrades.CloseSocket();
            GetSocket.CloseSocket();
            Thread.Sleep(50);
            MThread.AbortAll();
        }
    }
}

