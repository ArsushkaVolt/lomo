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
using System.Windows.Threading;


namespace WpfApp1
{

    public partial class MainWindow : Window
    {
        private List<PointData> currentPoints = new List<PointData>();
        private List<PointData> filteredPoints = new List<PointData>();
        private string currentRole = "Оператор";

        public MainWindow()
        {
            InitializeComponent();
            filteredPoints = new List<PointData>();
            Login();
            ApplyRoleRestrictions();
            UpdateModeUI();
        }

        private void Login()
        {
            var loginWin = new LoginWindow();
            if (loginWin.ShowDialog() == true)
            {
                currentRole = loginWin.CurrentRole;
            }
        }

        private void ApplyRoleRestrictions()
        {
            bool isMetrologistOrHigher = currentRole == "Метролог" || currentRole == "Администратор";
            // Здесь можно добавлять больше ограничений
        }

        #region Переключение режимов

        private void chkMetrologist_Checked(object sender, RoutedEventArgs e)
        {
            if (currentRole != "Администратор")
            {
                chkMetrologist.IsChecked = false;
                MessageBox.Show("Только Администратор может активировать режим Метролог.", "Доступ запрещён");
                return;
            }
            currentRole = "Метролог";
            ApplyRoleRestrictions();
            UpdateModeUI();
        }

        private void chkMetrologist_Unchecked(object sender, RoutedEventArgs e)
        {
            currentRole = "Оператор";
            ApplyRoleRestrictions();
            UpdateModeUI();
        }

        private void Mode_Operator_Click(object sender, RoutedEventArgs e) => SetRole("Оператор");
        private void Mode_Metrologist_Click(object sender, RoutedEventArgs e) => SetRole("Метролог");
        private void Mode_Admin_Click(object sender, RoutedEventArgs e) => SetRole("Администратор");

        private void SetRole(string role)
        {
            currentRole = role;
            ApplyRoleRestrictions();
            UpdateModeUI();
        }

        private void UpdateModeUI()
        {
            statusMode.Text = $"Пользователь: {currentRole}";
        }

        #endregion

        #region Измерение

        private void BtnFixPoint_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            double x = Math.Round(rand.NextDouble() * 200 - 50, 4);
            double y = Math.Round(rand.NextDouble() * 150 - 30, 4);

            var point = new PointData(x, y);
            currentPoints.Add(point);
            filteredPoints.Add(point);

            lstMeasuredPoints.Items.Add($"Точка {currentPoints.Count}: X={x}  Y={y}");

            txtCurrentCoord.Text = $"X = {x}   Y = {y}";
            UpdateMeasurementInfo();
        }

        private void BtnFilterOutliers_Click(object sender, RoutedEventArgs e)
        {
            if (currentRole != "Метролог" && currentRole != "Администратор")
            {
                ShowNotification("Доступ запрещён", "Фильтрация промахов доступна только в режиме Метролог", NotificationType.Error);
                return;
            }

            if (currentPoints.Count < 3)
            {
                ShowNotification("Недостаточно точек", "Для фильтрации промахов нужно минимум 3 точки", NotificationType.Warning);
                return;
            }

            filteredPoints = FilterOutliers(currentPoints);

            lstMeasuredPoints.Items.Clear();
            foreach (var p in filteredPoints)
            {
                lstMeasuredPoints.Items.Add($"Точка: X={p.X} Y={p.Y} (оставлена)");
            }

            txtOutliersInfo.Text = $"Отфильтровано промахов: {currentPoints.Count - filteredPoints.Count}";
            UpdateMeasurementInfo();

            ShowNotification("Фильтрация выполнена", $"Удалено {currentPoints.Count - filteredPoints.Count} промахов", NotificationType.Info);
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            filteredPoints = new List<PointData>(currentPoints);
            lstMeasuredPoints.Items.Clear();

            foreach (var p in currentPoints)
                lstMeasuredPoints.Items.Add($"Точка: X={p.X} Y={p.Y}");

            txtOutliersInfo.Text = "";
            UpdateMeasurementInfo();
        }

        private List<PointData> FilterOutliers(List<PointData> points)
        {
            if (points.Count < 3) return new List<PointData>(points);

            double avgX = points.Average(p => p.X);
            double stdDev = CalculateStdDev(points.Select(p => p.X));

            return points.Where(p => Math.Abs(p.X - avgX) / stdDev < 2.5).ToList();
        }

        private double CalculateStdDev(IEnumerable<double> values)
        {
            double avg = values.Average();
            double sumSq = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumSq / (values.Count() - 1));
        }

        private void BtnFinishElement_Click(object sender, RoutedEventArgs e)
        {
            if (currentPoints.Count == 0)
            {
                MessageBox.Show("Нет зафиксированных точек!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string type = (cmbElementType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Элемент";

            MessageBox.Show($"Элемент «{type}» успешно завершён!\nТочек (после фильтрации): {filteredPoints.Count}\nСКО: {CalculateSKO():F3} мм",
                            "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

            treeElements.Items.Add(new TreeViewItem
            {
                Header = $"● {type} ({filteredPoints.Count} точек)"
            });

            currentPoints.Clear();
            filteredPoints.Clear();
            lstMeasuredPoints.Items.Clear();
            txtCurrentCoord.Text = "X=0,0000  Y=0,0000";
            txtOutliersInfo.Text = "";
            UpdateMeasurementInfo();
        }

        private void UpdateMeasurementInfo()
        {
            int count = filteredPoints.Count > 0 ? filteredPoints.Count : currentPoints.Count;
            string type = (cmbElementType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Элемент";

            txtMeasurementInfo.Text = $"Тип: {type} | Точек: {count} | СКО: {CalculateSKO():F3} мм";
        }

        private double CalculateSKO()
        {
            var pointsToUse = filteredPoints.Count > 0 ? filteredPoints : currentPoints;
            if (pointsToUse.Count < 2) return 0.0;

            double avgX = pointsToUse.Average(p => p.X);
            return Math.Sqrt(pointsToUse.Average(p => Math.Pow(p.X - avgX, 2)));
        }

        #endregion

        #region Другие окна

        private void BtnOpenCS_Click(object sender, RoutedEventArgs e)
        {
            var win = new CoordinateSystemWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        #endregion

        #region Уведомления

        private void ShowNotification(string message, string details = "", NotificationType type = NotificationType.Warning)
        {
            var notification = new NotificationPanel(message, details, type);
            notificationPanel.Children.Add(notification);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(6) };
            timer.Tick += (s, e) =>
            {
                notificationPanel.Children.Remove(notification);
                timer.Stop();
            };
            timer.Start();
        }

        #endregion
    }

    public class PointData
    {
        public double X { get; }
        public double Y { get; }
        public PointData(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}