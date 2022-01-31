using Connector.Logs;
using Managers;
using QuikControl;
using System.Threading;
using QuikConnector.libs;
using QuikConnector.ServiceMessage.Package;
using QuikConnector.ServiceMessage.Message;
using ServiceMessage;
using System.Text;
using QuikConnector.libs.zlib;
using System.Diagnostics;

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
        private CSocket SendSocket = new CSocket(MAX_SIZE_MESSAGE);
        /// <summary> Сокет приема базовых сообщений </summary>
        private CSocket GetSocket = new CSocket(MAX_SIZE_MESSAGE);

        /// <summary> Стек сообщений на отправку </summary>
        private MQueue<string> QueueSend = new MQueue<string>();
        private MQueue<string> QueueGet = new MQueue<string>();

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
                var msg = Type + MsgServer.SP_FORSERVER + string.Join(MsgServer.SP_FORSERVER.ToString(), msgSend);
                QueueSend.Add(msg);
                QDebug.write(msg);
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
                var msg = Type + MsgServer.SP_FORSERVER + msgSend;
                QueueSend.Add(msg);
                QDebug.write(msg);
            }
        }
        /// <summary> Добавить сообщение на отправку серверу </summary>
        /// <param name="msgSend"></param>
        public void Send(string msgSend)
        {
            if (!msgSend.Empty())
            {
                QueueSend.Add(msgSend);
                QDebug.write(msgSend);
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
                    QDebug.write(msgSend);
                }
            }
        }

        /// <summary> Отправка сообщения  </summary>
        private void AllSocketConnected()
        {
            //Запускаем слушатель сокета
            Send(Commands.SIGNAL_CONNECT_SUCCESS, new string[] { "1" });
        }

        /// <summary> Функция осуществляет подключение к скрипту LUA, запущенный в терминале. </summary>
        /// <param name="ServerAddr">Адрес подключения (по умолчанию localhost)</param>
        /// <param name="port">Порт подключения (по умолчанию 8080)</param>
        /// <returns></returns>
        public bool ConnectSockets(string ServerAddr, int portSend, int portReceive)
        {
            //Сокет на отправку
            if (!SendSocket.CreateSocket(ServerAddr, portSend, false))
            {
                return false;
            }
            //Сокет на получение
            if (!GetSocket.CreateSocket(ServerAddr, portReceive))
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
                    Thread.Sleep(1);
                }
                mm.GetSocket.CloseSocket();
            });
            //Основной цикл обработки и отправки пакетов
            MThread.InitThread(ThreadPriority.Normal, this, (classMM) =>
            {
                MManager mm = (MManager)classMM;
                while (LoopProcessing)
                {
                    mm.HandlerPack();
                    Thread.Sleep(1);
                }
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
        /// <summary>
        ///  Распределение полученных сообщений из сокета
        /// </summary>
        /// <param name="content">Принятые данные</param>
        /// <param name="isHistoryTrades"> флаг что это историческая сделка </param>
        /// <returns></returns>
        private int Allocation(byte[] data)
        {
            if (data.Length > 0)
            {
                string content = Zlib.Unzip(data, 2000000, "windows-1251");
                QueueGet.Add(content);
            }
            return data.Length;
        }

        public delegate void ActivatorEvents();
        /// <summary> Событие которое позволяет прогрузить события в очереди. Чтоб избежать застоя. </summary>
        public event ActivatorEvents AcivateAllEvent;

        private void HandlerPack()
        {
            if (QueueGet.Count > 0)
            {
                string content = QueueGet.getFirst;
                QueueGet.DeleteItem(content);
                PackMsg pack = PackMsg.Create(content);
                pack.Each((msg) =>
                {
                    QDebug.write(msg.Content());
                    HandlerMessage(msg);
                });
                AcivateAllEvent();
            }

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
                        if (report.Value.ActivateDate)
                        {
                            AcivateAllEvent();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
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

        /// <summary> Функция обработки отправки сообщений </summary>
        /// <param name="contentMsg">Поступающее сообщение</param>
        private void ProcessSend()
        {
            if (QueueSend.Count > 0)
            {
                var item = QueueSend.getFirst;
                QueueSend.DeleteItem(item);
                var bytes = SendSocket.Send(Encoding.ASCII.GetBytes(item + '\t'));
            }
        }

        /// <summary> Закрыть соединение и прекратить передачу сообщений</summary>
        public void Close()
        {
            StopGettingData();
            LoopProcessing = false;

            SendSocket.CloseSocket();
            GetSocket.CloseSocket();
            Thread.Sleep(50);
            MThread.AbortAll();
        }
    }
}

