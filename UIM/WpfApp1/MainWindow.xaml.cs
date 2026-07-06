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

namespace UIM.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void Calibration_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void Logout_Click(object sender, RoutedEventArgs e)
        {

        }
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}