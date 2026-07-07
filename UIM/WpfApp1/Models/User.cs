using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public enum UserRole
    {
        Operator,      // Упрощённый режим
        Metrologist,   // Полный доступ + калибровка
        Administrator  // Управление пользователями и Audit Trail
    }

    /// <summary>
    /// Пользователь системы
    /// </summary>
    public class User
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public string Login { get; set; }
    }
}
