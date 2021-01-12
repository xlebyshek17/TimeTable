using System;

namespace Server
{
    public class Program
    {
        public static int Main(String[] args)
        {
            // Создаем объект класса Listener для прослушивания подключений
            Listener listener = new Listener();

            // Начинаем прослушивание 
            listener.StartListening();
            return 0;
        }
    }
}
