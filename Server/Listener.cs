using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Listener
    {
        // Сигнал потока 
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        // Прослушивающий сокет
        public static Socket listener;

        // Установливаем локальную конечную точку для сокета
        static IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        static IPAddress ipAddress = ipHostInfo.AddressList[0];
        static IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
        

        public Listener()
        {
            // Создаем TCP/IP сокет 
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening()
        { 
            try
            {
                // Связываем сокет с локальной конечной точкой
                listener.Bind(localEndPoint);

                // Прослушиваем входящие соединения. Максимальное количество подключений - 50
                listener.Listen(50);

                while (true)
                {
                    // Устанавливаем несигнальное состояние потока
                    allDone.Reset();

                    // Запускаем асинхронный сокет для прослушивания соединений
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Прежде чем продолжить, дождёмся установления соединения
                    allDone.WaitOne(); 
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Сигнал основному потоку, чтобы он продолжил работу.
            allDone.Set();

            // Получаем сокет, который обрабатывает запрос клиента
            Socket handler = listener.EndAccept(ar);

            Console.WriteLine("Connection completed");

            // Cоздаем сокет, с которого будем получать данные
            ClientController.AddClient(handler);

            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
        }
    }
}
