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
using WpfApp1.ViewModels;

namespace WpfApp1
{

    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        // ==================== ОБРАБОТЧИКИ МЕНЮ ====================

        private void Calibration_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Калибровка доступна только в режиме Метролог или Администратор.",
                "Калибровка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти из текущей учётной записи?", "Выход",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ApplicationState.Logout();

                var loginWindow = new LoginWindow();
                Application.Current.MainWindow = loginWindow;
                loginWindow.Show();
                this.Close();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ElementViewModel element)
            {
                _vm.SelectedElement = element;
            }
        }
    }
}