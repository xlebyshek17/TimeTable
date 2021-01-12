using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ReceivePackage
    {
        // Размер получаемого сообщения.
        public const int BufferSize = 1024;
        // Полученное сообщение. 
        public byte[] buffer = new byte[BufferSize];
        // Данные о клиенте
        private Socket client;
        private int Id;
        private string name;

        public ReceivePackage(Socket receiveSocket, int id)
        {
            client = receiveSocket;
            Id = id;
        }

        public void StartReceiving()
        {
            try
            {
                // Начинаем принимать данные от клиента
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            // Строка для данных, отправляемых клиенту
            string result = string.Empty;
            try
            {
                // Полученная строка данных
                StringBuilder sb = new StringBuilder();
                // Считываем данные от клиента
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 1)
                {
                    // Сохраняем в строку полученные данные 
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    string[] words = sb.ToString().Split();
                    if (words[0] == "name")
                        setName(words);
                    else
                    {
                        // Выводим на экран данные, полученные от клиента
                        Console.WriteLine("\nRead {0} bytes from client {1}. \n Data : {2}", sb.ToString().Length, name, sb.ToString());
                        // Обрабатываем запрос клиента
                        result = requestProcessing(words, result);
                        // Отправляем ответ клиенту
                        Sender send = new Sender(client, result, name);
                        send.Send();
                    }
                    // Продолжаем получать данные
                    StartReceiving();
                }
            }
            catch
            {
                // Если клиент отключен, вызываем метод для закрытия сокета
                if (!client.Connected)
                {
                    Disconnect(client);
                }
                // В ином случаем, начинаем получать данные дальше
                else
                {
                    StartReceiving();
                }
            }
        }

        // Метод для закрытия сокета клиента
        private void Disconnect(Socket client)
        {
            client.Disconnect(true);
            ClientController.RemoveClient(Id);
            Console.WriteLine("The client {0} is disconnected", name);
        }

        // Присваиваем имя клиенту
        private void setName(string[] words)
        {
            name = words[1];
            Console.WriteLine("*** client name: {0}", name);
        }

        // Обработка запроса клиента
        private string requestProcessing(string[] words, string result)
        {
            foreach (string line in File.ReadLines(@"D:\10710218\KsisKursach\timeTable.txt", Encoding.UTF8))
            {
                if (words[0] == "All")
                    result += line;
                else
                {
                    int count = 0;
                    for (int i = 0; i < words.Length - 1; i++)
                    {
                        if (line.Contains(words[i]))
                        {
                            count++;
                        }
                    }

                    if (count == words.Length - 1)
                    {
                        result += line;
                    }
                }
            }
            if (result == "")
                result = "Ошибка запроса!";
            return result;
        }
    }
}
