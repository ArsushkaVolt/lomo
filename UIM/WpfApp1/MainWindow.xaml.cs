using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIM.UI.WPF.Views;

namespace UIM.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selected    =  sender as RadioButton;

            switch (selected.Name)
            {
                case "Oper":
                    break;

                case "Metr":
                    break;

                case "Adm":
                    break;
            }
        }
        public void Calibration_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            this.Show();
        }
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}