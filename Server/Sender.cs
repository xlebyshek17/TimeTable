using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Sender
    {
        public static Socket client;
        public static string data;
        public static string name;

        public Sender(Socket _client, string _data, string _name)
        {
            client = _client;
            data = _data;
            name = _name;
        }

        public void Send()
        {
            // Преобразуем строку данных с использованием кодировки UTF8.
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Начинаем отправлять данные клиенту
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Завершаем отправку данных клиенту  
                int bytesSent = client.EndSend(ar);
                // Выводим на экран данные, отправлемые клиенту
                Console.WriteLine("Sent {0} bytes to client {1}.\n Data : {2}", bytesSent, name, data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
