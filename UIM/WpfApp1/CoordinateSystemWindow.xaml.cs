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
    /// Логика взаимодействия для CoordinateSystemWindow.xaml
    /// </summary>
    public partial class CoordinateSystemWindow : Window
    {
        public CoordinateSystemWindow()
        {
            InitializeComponent();
            LoadDemoData();
        }

        private void LoadDemoData()
        {
            var psk = new TreeViewItem { Header = "ПСК (Приборная) — жёлтая", IsExpanded = true };
            treeCS.Items.Add(psk);

            var workCS = new TreeViewItem { Header = "Рабочая СК 1 (зелёная)" };
            treeCS.Items.Add(workCS);
        }

        private void BtnAddCS_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Новая система координат создана.\n(Здесь будет диалог создания)",
                            "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDeleteCS_Click(object sender, RoutedEventArgs e)
        {
            if (treeCS.SelectedItem != null)
            {
                MessageBox.Show("СК удалена (с пересчётом зависимых)", "Удаление");
            }
        }

        private void BtnApplyTransform_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Трансформация применена.\nВсе зависимые измерения пересчитаны.",
                            "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
