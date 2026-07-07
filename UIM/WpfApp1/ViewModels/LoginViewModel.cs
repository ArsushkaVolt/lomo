using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Models;
namespace WpfApp1.ViewModels
{
    /// <summary>
    /// ViewModel окна авторизации
    /// </summary>
    public class LoginViewModel : ObservableObject
    {
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Асинхронная авторизация
        /// </summary>
        public async Task<bool> LoginAsync(string login, string password)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Введите логин и пароль";
                return false;
            }

            await Task.Delay(400); // имитация

            login = login.Trim().ToLower();

            // Демо-логика (по ТЗ разные роли)
            User user = login switch
            {
                "operator" when password == "123" => new User { FullName = "Петров П.П.", Role = UserRole.Operator },
                "metrolog" when password == "123" => new User { FullName = "Сидорова А.И.", Role = UserRole.Metrologist },
                "admin" when password == "123" => new User { FullName = "Иванов И.И.", Role = UserRole.Administrator },
                _ => null
            };

            if (user != null)
            {
                ApplicationState.CurrentUser = user;
                return true;
            }

            ErrorMessage = "Неверный логин или пароль";
            return false;
        }
    }
}