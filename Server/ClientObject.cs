using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    // Объект для асинхронного считывания клиентских данных
    public class ClientObject
    {  
        // Сокет клиента
        public Socket client { get; set; }
        // Поле для приема данных
        public ReceivePackage Receive { get; set; }
        // Id клиента
        public int Id { get; set; }

        // Конструктор класса ClientObject
        public ClientObject(Socket socket, int id)
        {
            // Создаем объект класса ReceivePackage для приема данных
            Receive = new ReceivePackage(socket, id);
            // Начинаем прием данных
            Receive.StartReceiving();
            // Иницилизируем клиента и id
            client = socket;
            Id = id;
        }
    }
}
