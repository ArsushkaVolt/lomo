using System;
using System.Collections.Generic;


namespace WpfApp1.Protocol
{
    // ================================================================
    // ОСНОВНОЙ КЛАСС ПРОТОКОЛА
    // ================================================================

    public class ProtocolData
    {
        public string ProtocolNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OperatorName { get; set; }
        public string DeviceSerial { get; set; }
        public string BatchNumber { get; set; }
        public uint SoftwareVersion { get; set; }

        public double CalibrationX { get; set; } = 1.0;
        public double CalibrationY { get; set; } = 1.0;
        public double AngleCorrection { get; set; } = 0.0;

        public List<MeasuredPoint> Points { get; set; }
        public List<GeometricElement> Elements { get; set; }
        public List<MeasurementResult> Results { get; set; }
        public List<FormDeviation> FormDeviations { get; set; }
        public List<PositionDeviation> PositionDeviations { get; set; }
        public ProtocolStatistics Statistics { get; set; }
        public List<AuditRecord> AuditTrail { get; set; }

        public ProtocolData()
        {
            Points = new List<MeasuredPoint>();
            Elements = new List<GeometricElement>();
            Results = new List<MeasurementResult>();
            FormDeviations = new List<FormDeviation>();
            PositionDeviations = new List<PositionDeviation>();
            AuditTrail = new List<AuditRecord>();
            Statistics = new ProtocolStatistics();
        }
    }

    // ================================================================
    // СТАТИСТИКА
    // ================================================================

    public class ProtocolStatistics
    {
        public int TotalPoints { get; set; }
        public int TotalElements { get; set; }
        public int TotalResults { get; set; }
        public int GoodCount { get; set; }
        public int BadCount { get; set; }
        public int FilteredPoints { get; set; }
        public double GoodPercentage => TotalResults > 0 ? (double)GoodCount / TotalResults * 100 : 0;
    }

    // ================================================================
    // АУДИТ
    // ================================================================

    public class AuditRecord
    {
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string IpAddress { get; set; }
    }

    // ================================================================
    // ТОЧКА
    // ================================================================

    public enum PointType { Measured, Calculated, Nominal, Filtered }

    public class MeasuredPoint
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public PointType Type { get; set; }
        public bool IsReadOnly { get; set; } = true;
        public DateTime Timestamp { get; set; }
        public double StandardDeviation { get; set; }
        public int FilteredOutCount { get; set; }
        public List<double> RawXValues { get; set; } = new();
        public List<double> RawYValues { get; set; } = new();
    }

    // ================================================================
    // ГЕОМЕТРИЧЕСКИЕ ЭЛЕМЕНТЫ
    // ================================================================

    public enum ElementType { Point, Line, Circle, Ellipse, Array }

    public abstract class GeometricElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ElementType ElementType { get; set; }
        public List<int> PointIds { get; set; } = new();
        public List<int> FilteredPointIds { get; set; } = new();
        public bool IsMeasured { get; set; }
        public double StandardDeviation { get; set; }
        public string Status { get; set; }
        public List<int> ParentElementIds { get; set; } = new();
        public List<int> ChildElementIds { get; set; } = new();
    }

    public class LineElement : GeometricElement
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double Angle { get; set; }
        public double Length { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public LineElement() => ElementType = ElementType.Line;
    }

    public class CircleElement : GeometricElement
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
        public double Diameter => 2 * Radius;
        public double MinRadius { get; set; }
        public double MaxRadius { get; set; }
        public double Roundness => MaxRadius - MinRadius;
        public CircleElement() => ElementType = ElementType.Circle;
    }

    public class EllipseElement : GeometricElement
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double MajorAxis { get; set; }
        public double MinorAxis { get; set; }
        public double Angle { get; set; }
        public double Eccentricity => MajorAxis > 0 ? Math.Sqrt((MajorAxis * MajorAxis - MinorAxis * MinorAxis) / (MajorAxis * MajorAxis)) : 0;
        public EllipseElement() => ElementType = ElementType.Ellipse;
    }

    // ================================================================
    // РЕЗУЛЬТАТЫ ИЗМЕРЕНИЙ
    // ================================================================

    public enum ResultType { Distance, Diameter, Radius, Angle, Length, Number, TrigValue }

    public class MeasurementResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ResultType Type { get; set; }
        public double Value { get; set; }
        public double Nominal { get; set; }
        public double Tolerance { get; set; }
        public double Deviation => Value - Nominal;
        public string Status { get; set; }
        public string BaseElementName { get; set; }
        public List<int> ElementIds { get; set; } = new();
        public string Modifier { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // ================================================================
    // ОТКЛОНЕНИЯ ФОРМЫ
    // ================================================================

    public enum FormDeviationType { Straightness, Roundness, Ellipticity }

    public class FormDeviation
    {
        public int Id { get; set; }
        public FormDeviationType Type { get; set; }
        public double MaxDeviation { get; set; }
        public double MinDeviation { get; set; }
        public double Range { get; set; }
        public string Status { get; set; }
        public int ElementId { get; set; }
        public double Tolerance { get; set; }
    }

    // ================================================================
    // ОТКЛОНЕНИЯ РАСПОЛОЖЕНИЯ
    // ================================================================

    public enum PositionDeviationType
    {
        Parallelism,
        Perpendicularity,
        Concentricity,
        Symmetry,
        Runout,
        Position,
        Inclination
    }

    public class PositionDeviation
    {
        public int Id { get; set; }
        public PositionDeviationType Type { get; set; }
        public double Value { get; set; }
        public double Nominal { get; set; }
        public double Tolerance { get; set; }
        public double Deviation { get; set; }
        public string Status { get; set; }
        public int BaseElementId { get; set; }
        public int ControlledElementId { get; set; }
        public string Modifier { get; set; }
    }
}