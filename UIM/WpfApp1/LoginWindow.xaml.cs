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
using WpfApp1.ViewModels;

namespace WpfApp1
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel = new LoginViewModel();

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text?.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Блокировка кнопки во время проверки
           

            bool success = await _viewModel.LoginAsync(login, password);

            if (success)
            {
                // Закрываем окно логина и открываем главное окно
                MainWindow mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.\n\nДля теста используйте:\nЛогин: admin\nПароль: 123",
                    "Ошибка авторизации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
