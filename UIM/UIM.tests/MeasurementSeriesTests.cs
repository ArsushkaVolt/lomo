using NUnit.Framework;
using UIM.core.Geometry;
using UIM.core.Geometry.Point2D_4_1;

namespace UIM.Tests;
public class MeasurementSeriesTests
{
    // проверка добавления точки
    [Test]
    public void AddOneMeasurementTest()
    {
        var series = new MeasurementSeries();

        series.AddMeasurement(new Point2D(10, 20));

        Point2D average = series.GetAverage();

        Assert.That(average.X, Is.EqualTo(10));
        Assert.That(average.Y, Is.EqualTo(20));
    }

    // проверка среднего значения
    [Test]
    public void AverageCalculationTest()
    {
        var series = new MeasurementSeries();

        series.AddMeasurement(new Point2D(2, 4));
        series.AddMeasurement(new Point2D(4, 8));

        Point2D average = series.GetAverage();

        Assert.That(average.X, Is.EqualTo(3));
        Assert.That(average.Y, Is.EqualTo(6));
    }

    // проверка пустой серии
    [Test]
    public void AverageEmptySeriesTest()
    {
        var series = new MeasurementSeries();

        Point2D average = series.GetAverage();

        Assert.That(average.X, Is.EqualTo(0));
        Assert.That(average.Y, Is.EqualTo(0));
    }

    // проверка СКО для одинаковых точек
    [Test]
    public void SkoSamePointsTest()
    {
        var series = new MeasurementSeries();

        series.AddMeasurement(new Point2D(5, 5));
        series.AddMeasurement(new Point2D(5, 5));
        series.AddMeasurement(new Point2D(5, 5));

        var sko = series.GetSko();

        Assert.That(sko.SkoX, Is.EqualTo(0));
        Assert.That(sko.SkoY, Is.EqualTo(0));
    }

    // проверка СКО
    [Test]
    public void SkoCalculationTest()
    {
        var series = new MeasurementSeries();

        series.AddMeasurement(new Point2D(1, 1));
        series.AddMeasurement(new Point2D(2, 2));
        series.AddMeasurement(new Point2D(3, 3));

        var sko = series.GetSko();

        Assert.That(sko.SkoX, Is.EqualTo(1).Within(0.0001));
        Assert.That(sko.SkoY, Is.EqualTo(1).Within(0.0001));
    }

    // проверка очистки
    [Test]
    public void ClearSeriesTest()
    {
        var series = new MeasurementSeries();

        series.AddMeasurement(new Point2D(10, 10));
        series.AddMeasurement(new Point2D(20, 20));

        series.Clear();

        Point2D average = series.GetAverage();

        Assert.That(average.X, Is.EqualTo(0));
        Assert.That(average.Y, Is.EqualTo(0));
    }

    // проверка одной точки
    [Test]
    public void SkoOnePointTest()
    {
        var series = new MeasurementSeries();

        series.AddMeasurement(new Point2D(5, 7));

        var sko = series.GetSko();

        Assert.That(sko.SkoX, Is.EqualTo(0));
        Assert.That(sko.SkoY, Is.EqualTo(0));
    }
}