using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TimeTable
{
    // Класс-объект для приема данных с удаленного устройства.
    public class ClientObject
    {
        // Сокет клиента  
        public Socket client = null;
        // Размер получаемого сообщения.  
        public const int BufferSize = 6500;
        // Полученное сообщение.  
        public byte[] buffer = new byte[BufferSize];
    }
}
