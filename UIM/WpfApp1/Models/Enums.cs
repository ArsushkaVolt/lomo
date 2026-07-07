using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    // Models/Enums.cs
    public enum ElementType
    {
        Point,
        ArrayOfPoints,
        Line,
        Circle,
        Ellipse,
        // ... другие по Приложению Б
    }

    public enum ElementOrigin
    {
        Measured,     // Измеренный
        Calculated,   // Рассчитанный
        Nominal       // Номинальный (теоретический)
    }
}
