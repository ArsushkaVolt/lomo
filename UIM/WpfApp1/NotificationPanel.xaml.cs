using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    public partial class NotificationPanel : UserControl
    {
        public NotificationPanel(string message, string details = "", NotificationType type = NotificationType.Warning)
        {
            InitializeComponent();
            txtMessage.Text = message;
            txtDetails.Text = details;

            switch (type)
            {
                case NotificationType.Warning:
                    Background = new SolidColorBrush(Color.FromRgb(255, 243, 205));
                    BorderBrush = new SolidColorBrush(Color.FromRgb(255, 234, 160));
                    break;
                case NotificationType.Error:
                    Background = new SolidColorBrush(Color.FromRgb(248, 215, 218));
                    BorderBrush = new SolidColorBrush(Color.FromRgb(220, 53, 69));
                    break;
                case NotificationType.Info:
                    Background = new SolidColorBrush(Color.FromRgb(207, 226, 255));
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 123, 255));
                    break;
            }
        }
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}