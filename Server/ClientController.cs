using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public static class ClientController
    {
        // Список клиентов
        public static List<ClientObject> Clients = new List<ClientObject>();
        
        // Добавление клиента клиента в список
        public static void AddClient(Socket socket)
        {
            Clients.Add(new ClientObject(socket, Clients.Count));
        }

        // Удаление клиента из списка
        public static void RemoveClient(int id)
        {
            Clients.RemoveAt(Clients.FindIndex(x => x.Id == id));
        }
    }
}
