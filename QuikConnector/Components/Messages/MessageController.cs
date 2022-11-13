using System.Threading;
using System.Text;
using QuikConnector.libs.zlib;
using QuikConnector.Components.Terminal;
using QuikConnector.Components.Debug;
using QuikConnector.libs.net;
using QuikConnector.libs.queue;
using QuikConnector.Components.Controllers;

namespace QuikConnector.Components.Messages
{
    /// <summary> Менеджер сообщений </summary>
    public class MessageController
    {
        /// <summary> Разделитель данных в сообщении для сервера </summary>
        const string GLUE_SERVER = "|";
        /// <summary> Флаг работы основного цикла. </summary>
        public static bool LoopProcessing = true;
        /// <summary> Размер принимаемого сообщения от сервера. </summary>
        private const int MAX_SIZE_MESSAGE = 10000000;

        /// <summary> Сокет отправки </summary>
        private readonly Socket SendSocket = new Socket(MAX_SIZE_MESSAGE);
        /// <summary> Сокет приема базовых сообщений </summary>
        private readonly Socket GetSocket = new Socket(MAX_SIZE_MESSAGE);

        /// <summary> Стек сообщений на отправку </summary>
        private readonly Queue<string> QueueSend = new Queue<string>();
        private readonly Queue<MessagePackage> QueuePack = new Queue<MessagePackage>();
        private readonly Queue<Message> QueueNotPriority = new Queue<Message>();

        /// <summary> Объект конвертора </summary>
        public MessageParser Convertor = null;

        /// <summary> Конструктор объекста Менеджер сообщений </summary>
        /// <param name="trader"></param>
        public MessageController(TerminalControl trader)
        {
            if (!trader.Empty())
            {
                Convertor = new MessageParser(trader);
            }
        }

        /// <summary> Добавить сообщение на отправку серверу  </summary>
        /// <param name="Type">Тип сообщения или заголовок.</param>
        /// <param name="msgSend">Тело сообщения(само сообщение)</param>
        public void Send(string Type, string[] msgSend)
        {
            if (!msgSend.Empty())
            {
                var msg = Type + GLUE_SERVER + string.Join(GLUE_SERVER, msgSend);
                QueueSend.Add(msg);
                QDebug.Output(msg);
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
                var msg = Type + GLUE_SERVER + msgSend;
                QueueSend.Add(msg);
                QDebug.Output(msg);
            }
        }
        /// <summary> Добавить сообщение на отправку серверу </summary>
        /// <param name="msgSend"></param>
        public void Send(string msgSend)
        {
            if (!msgSend.Empty())
            {
                QueueSend.Add(msgSend);
                QDebug.Output(msgSend);
            }
        }
        /// <summary> Добавить сообщение на отправку серверу с проверкой предыдущей, если совпадает то не добавляет. </summary>
        /// <param name="msgSend"> Отправляемое сообщение </param>
        public void SendCheckLast(string msgSend)
        {
            if (!msgSend.Empty() && msgSend != QueueSend.Last)
            {
                QueueSend.Add(msgSend);
                QDebug.Output(msgSend);
            }
        }

        /// <summary> Отправка сообщения  </summary>
        private void AllSocketConnected()
        {
            //Запускаем слушатель сокета
            Send(MessageServer.CONNECT_SUCCESS, new string[] { "1" });
        }

        /// <summary> Функция осуществляет подключение к скрипту LUA, запущенный в терминале. </summary>
        /// <param name="ServerAddr">Адрес подключения (по умолчанию localhost)</param>
        /// <param name="port">Порт подключения (по умолчанию 8080)</param>
        /// <returns></returns>
        public bool ConnectSockets(string ServerAddr, int portSend, int portReceive)
        {
            //Сокет на отправку
            if (!SendSocket.CreateSocket(ServerAddr, portSend))
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
            ThreadsController.InitThread(ThreadPriority.Normal, this, (classMM) =>
            {
                MessageController mm = (MessageController)classMM;
                while (LoopProcessing)
                {
                    //Получает данные с сокета
                    mm.Scaning();
                    Thread.Sleep(1);
                }
                mm.GetSocket.CloseSocket();
            });
            //Основной цикл обработки и отправки пакетов
            ThreadsController.InitThread(ThreadPriority.Normal, this, (classMM) =>
            {
                MessageController mm = (MessageController)classMM;
                while (LoopProcessing)
                {
                    mm.HandlerPack();
                    Thread.Sleep(1);
                }
            });
            //Основной цикл обработки и отправки сообщений
            ThreadsController.InitThread(ThreadPriority.Normal, this, (classMM) =>
            {
                MessageController mm = (MessageController)classMM;
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
            SendCheckLast("Stop" + GLUE_SERVER + "1");
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
                QueuePack.Add(MessagePackage.Parse(content));
            }
            return data.Length;
        }

        private void HandlerPack()
        {
            int k = 0;
            if (QueuePack.Count > 0)
            {
                var pack = QueuePack.GetFirst;
                QueuePack.DeleteItem(pack);
                bool wasPriority = false;
                foreach (var textMsg in pack.GetData())
                {
                    var msg = Message.Create(textMsg);
                    QDebug.Output(textMsg);
                    //Сортируем на приоритетные и нет
                    if (!MessagePriority.NotPriority(msg))
                    {
                        HandlerMessage(msg);
                        wasPriority = true;
                        k++;
                    }
                    else
                    {
                        QueueNotPriority.Add(msg);
                    }
                }
                if (wasPriority)
                {

                }
                if (k > 0)
                {
                    QDebug.Output("prior " + k.ToString());
                }
            }
            else
            {
                //Обработка не приоритетных сообщений
                while (QueuePack.Count == 0 && QueueNotPriority.Count > 0)
                {
                    var npMsg = QueueNotPriority.GetFirst;
                    QueueNotPriority.DeleteItem(npMsg);
                    HandlerMessage(npMsg);
                    k++;
                }
                if (k > 0)
                {
                    QDebug.Output("not_prior " + k.ToString());
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool HandlerMessage(Message message)
        {
            if (message.NotIsNull())
            {
                var report = Convertor.NewMessage(message);
                if (report.NotIsNull())
                {
                    //Отладочная информация
                    if (report.Value.Reply.NotIsNull() && report.Value.Reply.Length > 0)
                    {
                        QDebug.Output(report.Value.Reply);
                    }
                    //Проверяем надо ли отправить ответ на сервер
                    if (report.Value.Response.NotIsNull() && report.Value.Response.Length > 0)
                    {
                        SendCheckLast(string.Join(GLUE_SERVER, report.Value.Response));
                    }
                }
                return true;
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
                var item = QueueSend.GetFirst;
                QueueSend.DeleteItem(item);
                SendSocket.Send(Encoding.ASCII.GetBytes(item + '\t'));
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
            ThreadsController.AbortAll();
        }
    }
}

