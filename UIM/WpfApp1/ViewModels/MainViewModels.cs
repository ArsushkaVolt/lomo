using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public User CurrentUser => ApplicationState.CurrentUser;

        public string CurrentModeDisplay => CurrentUser?.Role switch
        {
            UserRole.Operator => "ОПЕРАТОР",
            UserRole.Metrologist => "МЕТРОЛОГ",
            UserRole.Administrator => "АДМИНИСТРАТОР",
            _ => "—"
        };

        public Brush ModeColor => CurrentUser?.Role switch
        {
            UserRole.Operator => Brushes.Blue,
            UserRole.Metrologist => Brushes.DarkGreen,
            UserRole.Administrator => Brushes.Purple,
            _ => Brushes.Black
        };

        public bool IsMetrologistOrHigher => CurrentUser?.Role is UserRole.Metrologist or UserRole.Administrator;

        public ObservableCollection<ElementViewModel> RootElements { get; } = new();
        public ElementViewModel SelectedElement { get; set; }

        public string AuditStatus { get; set; } = "● Audit Trail активен";

        public MainViewModel()
        {
            AddDemoElements();   
        }

        /// <summary>
        /// Добавление демо-элементов для тестирования TreeView
        /// </summary>
        public void AddDemoElements()
        {
            var line = new ElementViewModel("Базовая прямая", ElementType.Line, ElementOrigin.Measured)
            {
                PointCount = 12,
                Sko = 0.0018
            };

            var circle = new ElementViewModel("Отверстие Ø25.00", ElementType.Circle, ElementOrigin.Measured)
            {
                PointCount = 7,
                HasWarning = true,
                WarningText = "Минимальное кол-во точек"
            };

            var nominal = new ElementViewModel("Центр номинальный", ElementType.Point, ElementOrigin.Nominal);

            RootElements.Add(line);
            RootElements.Add(circle);
            RootElements.Add(nominal);
        }
    }
}