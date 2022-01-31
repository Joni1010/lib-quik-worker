using Connector.Logs;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuikConnector.libs
{
    public class CSocket
    {
        // State object for reading client data asynchronously
        public class StateObject : object
        {
            public static readonly byte[] RESPONSE_GET = new byte[] { 99, 99};
            // Client  socket.
            public Socket Socket;
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

        public StateObject StateSock = null;

        private IPAddress ipAddress;
        private IPEndPoint remoteEP;

        public CSocket(int sizeBuff)
        {
            this.StateSock = new StateObject(sizeBuff);
        }
        /// <summary>
        /// Создает сокет
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool CreateSocket(string host, int port, bool tout = true)
        {
            var result = false;
            Qlog.CatchException(() =>
            {
#pragma warning disable CS0618 // Тип или член устарел
                IPHostEntry ipHostInfo = Dns.Resolve(host);//Dns.GetHostName()
#pragma warning restore CS0618 // Тип или член устарел
                //IPHostEntry ipHostInfo = Dns.Resolve(host);//Dns.GetHostName()
                this.ipAddress = ipHostInfo.AddressList[0];
                this.remoteEP = new IPEndPoint(ipAddress, port);

                this.StateSock.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // if (tout)
                {
                    this.StateSock.Socket.ReceiveTimeout = 500;
                }
                this.StateSock.Socket.ReceiveBufferSize = this.StateSock.BufferSize;
                this.StateSock.Socket.Connect(this.remoteEP);
                result = true;
            }, "", () =>
            {
                result = false;
            });
            return result;
        }

        /// <summary> Получает данные из сокета </summary>
        /// <param name="baseObj"></param>
        /// <returns></returns>
        /// 
        public int Receive()
        {
            if (this.StateSock.Socket.IsNull() || this.StateSock.Socket.Available <= 0)
            {
                return 0;
            }
            this.StateSock.BufferString = "";
            try
            {
                Array.Clear(StateSock.buffer, 0, StateSock.buffer.Length);
                StateSock.TransferBytes = StateSock.Socket.Receive(StateSock.buffer, StateSock.Socket.Available, SocketFlags.None);
                Send(Encoding.ASCII.GetBytes(StateSock.TransferBytes.ToString()));
                //StateSock.BufferString = Encoding.GetEncoding(1251).GetString(StateSock.buffer, 0, StateSock.TransferBytes);
            }
            catch (Exception)
            {
                int r = 0;
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
            if (this.StateSock.IsNull())
            {
                if (this.StateSock.Socket.NotIsNull())
                {
                    this.StateSock.Socket.Close();
                }
            }
        }
    }
}
