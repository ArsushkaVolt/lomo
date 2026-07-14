namespace WpfApp1.Models
{
    public enum UserRole
    {
        Operator,
        Metrologist,
        Administrator
    }

    public class User
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
