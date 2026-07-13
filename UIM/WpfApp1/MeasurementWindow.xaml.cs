using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class MeasurementWindow : Window
    {
        private List<Point> points = new List<Point>();
        private Random rand = new Random(); // симуляция измерений

        public MeasurementWindow()
        {
            InitializeComponent();
        }

        private void BtnFixPoint_Click(object sender, RoutedEventArgs e)
        {
            // Симуляция измерения
            double x = Math.Round(rand.NextDouble() * 200 - 50, 4);
            double y = Math.Round(rand.NextDouble() * 150 - 30, 4);

            points.Add(new Point(x, y));
            lstPoints.Items.Add($"X={x}  Y={y}");

            txtCurrentX.Text = $"X = {x}";
            txtCurrentY.Text = $"Y = {y}";
            txtPointsCount.Text = $"Точек в элементе: {points.Count}";

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            if (points.Count < 2) return;

            double avgX = points.Average(p => p.X);
            double avgY = points.Average(p => p.Y);
            double sko = Math.Round(CalculateSKO(), 4);

            txtAverage.Text = $"Среднее: X={avgX:F4}  Y={avgY:F4}";
            txtSKO.Text = $"СКО: {sko} мм";
        }

        private double CalculateSKO()
        {
            // Простой расчёт СКО (для демонстрации)
            if (points.Count < 2) return 0;
            double avgX = points.Average(p => p.X);
            return Math.Sqrt(points.Average(p => Math.Pow(p.X - avgX, 2)));
        }

        private void BtnCancelLast_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count > 0)
            {
                points.RemoveAt(points.Count - 1);
                lstPoints.Items.RemoveAt(lstPoints.Items.Count - 1);
                txtPointsCount.Text = $"Точек в элементе: {points.Count}";
                UpdateStatistics();
            }
        }

        private void BtnFinishElement_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Элемент завершён!\nТочек: {points.Count}\nСКО: {CalculateSKO():F4} мм",
                            "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }

    public class Point
    {
        public double X { get; }
        public double Y { get; }
        public Point(double x, double y) { X = x; Y = y; }
    }
}
