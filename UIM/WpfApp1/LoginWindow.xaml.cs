using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class LoginWindow : Window
    {
        public string CurrentUser { get; private set; } = "Оператор";
        public string CurrentRole { get; private set; } = "Оператор";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim().ToLower();
            string pass = txtPassword.Password;

            if (login == "admin" && pass == "admin")
            {
                CurrentRole = "Администратор";
                CurrentUser = "Администратор";
            }
            else if (login == "metrologist" || (login == "operator" && pass == "123"))
            {
                CurrentRole = "Метролог";
                CurrentUser = "Метролог";
            }
            else
            {
                CurrentRole = "Оператор";
                CurrentUser = "Оператор";
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}