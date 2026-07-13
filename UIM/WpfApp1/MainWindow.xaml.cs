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

namespace WpfApp1
{

    public partial class MainWindow : Window
    {
        private string currentMode = "Оператор";

        public MainWindow()
        {
            InitializeComponent();
            UpdateModeUI();
        }

        // Галочка для быстрого переключения в Метролог
        private void chkMetrologist_Checked(object sender, RoutedEventArgs e)
        {
            currentMode = "Метролог";
            UpdateModeUI();
        }

        private void chkMetrologist_Unchecked(object sender, RoutedEventArgs e)
        {
            currentMode = "Оператор";
            UpdateModeUI();
        }

        // Переключение через меню
        private void SetMode(string mode)
        {
            currentMode = mode;
            chkMetrologist.IsChecked = (mode == "Метролог");
            UpdateModeUI();
        }

        private void Mode_Operator_Click(object sender, RoutedEventArgs e) => SetMode("Оператор");
        private void Mode_Metrologist_Click(object sender, RoutedEventArgs e) => SetMode("Метролог");
        private void Mode_Admin_Click(object sender, RoutedEventArgs e) => SetMode("Администратор");

        private void UpdateModeUI()
        {
            statusMode.Text = $"Режим: {currentMode}";

            if (currentMode == "Метролог")
            {
                statusText.Text = "Расширенные функции активны";
                statusText.Foreground = Brushes.Orange;
                Title = "ПО УИМ — Режим Метролог";
            }
            else if (currentMode == "Администратор")
            {
                statusText.Text = "Административный доступ";
                statusText.Foreground = Brushes.Red;
                Title = "ПО УИМ — Режим Администратор";
            }
            else
            {
                statusText.Text = "Готов к измерению";
                statusText.Foreground = Brushes.Green;
                Title = "ПО УИМ — Универсальный измерительный микроскоп";
            }
        }


    }

}