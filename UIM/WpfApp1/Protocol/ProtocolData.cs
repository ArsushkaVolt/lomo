using System;
using System.Collections.Generic;

namespace WpfApp1.Protocol
{
    /// <summary>
    /// Результат измерения (размер, диаметр, угол, расстояние...)
    /// </summary>
    public class MeasurementResult
    {
        /// <summary>
        /// Уникальный ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название (например, "Диаметр отверстия D1")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип результата
        /// </summary>
        public ResultType Type { get; set; }

        /// <summary>
        /// Фактическое значение
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Номинальное значение (с чертежа)
        /// </summary>
        public double Nominal { get; set; }

        /// <summary>
        /// Допуск (±)
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// Отклонение (Value - Nominal)
        /// </summary>
        public double Deviation => Value - Nominal;

        /// <summary>
        /// Статус: Годен / Брак
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Имя базового элемента (для связей)
        /// </summary>
        public string BaseElementName { get; set; }

        /// <summary>
        /// ID элементов, участвовавших в расчете
        /// </summary>
        public List<int> ElementIds { get; set; }

        /// <summary>
        /// Модификатор (из Приложения Б: "+", "-", "S", "В", "Н" и т.д.)
        /// </summary>
        public string Modifier { get; set; }

        /// <summary>
        /// Время расчета
        /// </summary>
        public DateTime Timestamp { get; set; }

        public MeasurementResult()
        {
            ElementIds = new List<int>();
        }
    }

    /// <summary>
    /// Типы результатов (Приложение Б ТЗ)
    /// </summary>
    public enum ResultType
    {
        /// <summary>Расстояние между точками/элементами</summary>
        Distance,

        /// <summary>Диаметр</summary>
        Diameter,

        /// <summary>Радиус</summary>
        Radius,

        /// <summary>Угол</summary>
        Angle,

        /// <summary>Длина отрезка</summary>
        Length,

        /// <summary>Числовой коэффициент</summary>
        Number

        /// <summary>Тригонометрическое значение</summary>
    }
}