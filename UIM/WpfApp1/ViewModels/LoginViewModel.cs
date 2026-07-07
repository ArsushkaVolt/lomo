using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    class LoginViewModel
    {
        private string _fullName { get; set; }
        private UserRole _role { get; set; }
        private string _login { get; set; }
        private string _password { get; set; }
    }
}
