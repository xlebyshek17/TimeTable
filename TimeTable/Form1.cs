using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeTable
{
    public partial class Form1 : Form
    {
        // Номер порта для сервера
        private const int port = 11000;

        // Для каждой асинхронной операции создаем объект класса ManualResetEvent
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        public static ManualResetEvent sendDone = new ManualResetEvent(false);
        public static ManualResetEvent receiveDone = new ManualResetEvent(false);
        
        // Сокет клиента
        public static Socket client;

        // Ответ от сервера
        private static string response = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        // Обработка кнопки поиска расписания по выбранным критериям
        private void findButton_Click(object sender, EventArgs e)
        {
            // Посылаем запрос о расписании клиенту
            Send(client, getRequest());
            sendDone.WaitOne();

            // Получает ответ от сервера
            Receive(client);
            receiveDone.WaitOne();

            // Выводим полученные данные в таблицу
            outputTimetable();
        }

        // Формируем запрос
        public string getRequest()
        {
            string request = "";
            
            // Считываем данные о группе, предмету и преподавателю
            string group = groupComboBox.Text;
            string subject = subjectComboBox.Text;
            string teacher = teacherComboBox.Text;

            // Формируем запрос
            string[] str = { group, subject, teacher };
            for (int i = 0; i < str.Length; i++)
                if (str[i] != "")
                    request += str[i] + " ";
            if (request == "")
                request = "All";

            return request;
        }

        // Вывод полученных от сервера данных
        public void outputTimetable()
        {
            if (response == "Ошибка запроса!")
                MessageBox.Show("В расписании нет ни одного совпадения!", response);
            else
            {
                string[] words = response.Split('.');
                timeTableGridView.Rows.Clear();
                for (int i = 0; i < words.Length - 1; i++)
                    timeTableGridView.Rows.Add(words[i].Split());
            }
            receiveDone.Reset();
        }

        public static void StartClient(string ip)
        { 
            try
            {
                // Устанавливаем удаленную конечную точку для сокета 
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Создаем TCP/IP сокет.  
                client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Подключаемся к серверу  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);

                // Блокирует основной поток до получения сигнала объектом
                connectDone.WaitOne();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!");
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Используем свойство  AsyncState для извлечения аргумента, 
                // который был передан в третьем параметре метода BeginConnect()          
                // Полученное значение явно приводим к типу Socket  
                client = (Socket)ar.AsyncState;

                // Завершает ожидающий аcинхронный запрос на подлючение
                client.EndConnect(ar);

                // Сообщаем основному потоку, что завершили установление соединения.
                // Устанавливаем сигнальное состояние объекта для продолжения выполнения ожидающих потоков
                connectDone.Set(); 
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!");
            }
        }

        public static void Receive(Socket client)
        {
            try
            {
                // Создаем объект для получения данных  
                ClientObject state = new ClientObject();
                state.client = client;

                // Начинаем получать данные с сервера  
                client.BeginReceive(state.buffer, 0, ClientObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!");
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Полученная строка данных
                StringBuilder sb = new StringBuilder();

                // Используем свойство  AsyncState для извлечения аргумента, 
                // который был передан в третьем параметре метода BeginReceive()          
                // Полученное значение явно приводим к типу ClientObject  
                ClientObject state = (ClientObject)ar.AsyncState;
                Socket client = state.client;

                // Считываем данные с сервера
                // метод EndReceive() возвращает количество полученных байтов.
                int bytesRead = client.EndReceive(ar); 

                if (bytesRead > 0)
                {
                    // Сохраняем в строку полученные данные  
                    sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                    // Получаем остальные данные  
                    client.BeginReceive(state.buffer, 0, ClientObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    
                }
                
                // Если все данные были получены, записываем их в ответ 
                if (sb.Length > 1)
                {
                    response = sb.ToString();
                }

                // Сигнал о том, что все байты были получены.
                receiveDone.Set();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!");
            }
        }

        public static void Send(Socket client, String data)
        {
            // Преобразуем строку данных с использованием кодировки UTF8.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Для асинхронной отправки данных серверу используем метод BeginSend().
            // Начинаем отправлять данные на сервер 
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Используем свойство  AsyncState для извлечения аргумента, 
                // который был передан в третьем параметре метода BeginSend()          
                // Полученное значение явно приводим к типу Socket
                Socket client = (Socket)ar.AsyncState;

                // метод EndSend() возвращает количество отправляемых байтов.
                int bytesSent = client.EndSend(ar);

                // Сигнал о том, что все байты были отправлены.
                sendDone.Set();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!");
            }
        }

        // Кнопка выхода
        private void exitButton_Click(object sender, EventArgs e)
        {
            // При нажатии на кнопку "выход" отключаем клиента
            client.Close();
            // закрываем форму
            Close();
        }
    }
}
