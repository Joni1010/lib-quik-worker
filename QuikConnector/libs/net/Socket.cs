using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QuikConnector.libs.net
{
    public class Socket
    {
        const int TIMEOUT = 500;
        // State object for reading client data asynchronously
        public class StateObject : object
        {
            public static readonly byte[] RESPONSE_GET = new byte[] { 99, 99 };
            // Client  socket.
            public System.Net.Sockets.Socket Socket;
            //public Socket wSocket = null;
            // Size of receive buffer.
            public int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = null;
            /// <summary> Кол-во полученнх байт </summary>
            public int TransferBytes = 0;
            /// <summary> Строка-результат </summary>
            public string BufferString = "";

            public StateObject(int sizeBuff)
            {
                BufferSize = sizeBuff;
                buffer = new byte[BufferSize];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public StateObject StateSock = null;
        /// <summary>
        /// 
        /// </summary>
        private IPAddress ipAddress;
        /// <summary>
        /// 
        /// </summary>
        private IPEndPoint remoteEP;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeBuff"></param>
        public Socket(int sizeBuff)
        {
            StateSock = new StateObject(sizeBuff);
        }
        /// <summary>
        /// Создает сокет
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool CreateSocket(string host, int port)
        {
#pragma warning disable CS0618 // Тип или член устарел
            IPHostEntry ipHostInfo = Dns.Resolve(host);//Dns.GetHostName()
#pragma warning restore CS0618 // Тип или член устарел
            //IPHostEntry ipHostInfo = Dns.Resolve(host);//Dns.GetHostName()
            ipAddress = ipHostInfo.AddressList[0];
            remoteEP = new IPEndPoint(ipAddress, port);

            StateSock.Socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = TIMEOUT,
                ReceiveBufferSize = StateSock.BufferSize
            };
            StateSock.Socket.Connect(remoteEP);
            return true;
        }

        /// <summary> Получает данные из сокета </summary>
        /// <param name="baseObj"></param>
        /// <returns></returns>
        /// 
        public int Receive()
        {
            if (StateSock.Socket.IsNull() || StateSock.Socket.Available <= 0)
            {
                return 0;
            }
            StateSock.BufferString = "";
            StateSock.TransferBytes = 0;
            try
            {
                Array.Clear(StateSock.buffer, 0, StateSock.buffer.Length);
                StateSock.TransferBytes = StateSock.Socket.Receive(StateSock.buffer, StateSock.Socket.Available, SocketFlags.None);
                Send(Encoding.ASCII.GetBytes(StateSock.TransferBytes.ToString()));
                //StateSock.BufferString = Encoding.GetEncoding(1251).GetString(StateSock.buffer, 0, StateSock.TransferBytes);
            }
            catch (Exception)
            {

            }
            return StateSock.TransferBytes;
        }

        /// <summary>  Отправка сообщений  </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int Send(byte[] data)
        {
            try
            {
                StateSock.Socket.Send(data, data.Length, 0);
                return data.Length;
            }
            catch (Exception)
            {

            }
            return 0;
        }

        /// <summary> 
        /// Закрывает подключение к сокету 
        /// </summary>
        public void CloseSocket()
        {
            if (StateSock.IsNull())
            {
                if (StateSock.Socket.NotIsNull())
                {
                    StateSock.Socket.Close();
                }
            }
        }
    }
}
