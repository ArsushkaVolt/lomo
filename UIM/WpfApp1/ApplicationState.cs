using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Глобальное состояние приложения (текущий пользователь)
    /// Используется для передачи информации о роли между окнами
    /// </summary>
    public static class ApplicationState
    {
        private static User _currentUser;

        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        public static User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                // Можно добавить событие OnUserChanged, если потребуется
            }
        }

        /// <summary>
        /// Проверка, авторизован ли пользователь
        /// </summary>
        public static bool IsLoggedIn => CurrentUser != null;

        /// <summary>
        /// Сброс состояния (при выходе)
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
