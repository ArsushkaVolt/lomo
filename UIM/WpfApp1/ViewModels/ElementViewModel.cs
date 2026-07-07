using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public enum ElementType { Point, ArrayOfPoints, Line, Circle, Ellipse }
    public enum ElementOrigin { Measured, Calculated, Nominal }

    /// <summary>
    /// Модель элемента для TreeView (соответствует п. 3.2 ТЗ)
    /// </summary>
    public class ElementViewModel : ObservableObject
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public ElementType Type { get; set; }
        public ElementOrigin Origin { get; set; }   // Измеренный / Рассчитанный / Номинальный

        public double? Sko { get; set; }
        public int PointCount { get; set; }

        public string StatusText { get; private set; }
        public Brush StatusBrush { get; private set; }
        public bool HasWarning { get; set; }         // Для "Без фильтрации" и т.п.
        public string WarningText { get; set; }

        /// <summary>
        /// Отображение "Т " для теоретических элементов
        /// </summary>
        public string DisplayName => (Origin == ElementOrigin.Nominal ? "Т " : "") + Name;

        public ObservableCollection<ElementViewModel> Children { get; } = new();

        public ElementViewModel(string name, ElementType type, ElementOrigin origin)
        {
            Name = name;
            Type = type;
            Origin = origin;
            UpdateStatus();
        }

        /// <summary>
        /// Обновление визуального статуса согласно ТЗ
        /// </summary>
        public void UpdateStatus()
        {
            if (Origin == ElementOrigin.Nominal)
            {
                StatusText = "Теоретический";
                StatusBrush = Brushes.Gray;
            }
            else if (HasWarning)
            {
                StatusText = "⚠ " + (WarningText ?? "Без фильтрации");
                StatusBrush = Brushes.OrangeRed;
            }
            else
            {
                StatusText = Sko.HasValue ? $"СКО: {Sko:F4}" : "OK";
                StatusBrush = Brushes.DarkGreen;
            }
        }
    }
}
