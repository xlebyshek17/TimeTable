using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeTable
{
    public partial class UserEntrance : Form
    {
        public UserEntrance()
        {
            InitializeComponent();
        }

        // Кнопка для вызова следующей формы и подключения клиента
        private void EnterButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (loginTextBox.Text == "")
                {
                    MessageBox.Show("Пожалуйста, введите ваше имя!", "Ошибка входа");
                }
                if (ipTextBox.Text == "")
                {
                    MessageBox.Show("Пожалуйста, введите ip сервера!", "Ошибка входа");
                }

                else
                {
                    // Соединение клиента с сервером
                    Form1.StartClient(ipTextBox.Text);
                    // Посылаем серверу имя клиента
                    Form1.Send(Form1.client, "name " + loginTextBox.Text);
                    Form1.sendDone.WaitOne();

                    // При успешном подключении к серверу, открываем основную форму программы
                    Form1 form1 = new Form1();
                    Hide();
                    form1.ShowDialog();
                    Close();
                }   
            }
            catch (Exception)
            {
                MessageBox.Show("Введите ip-адрес существующего сервера!", "Ошибка!");
            }
        }
    }
}
