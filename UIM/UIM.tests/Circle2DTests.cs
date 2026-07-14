using NUnit.Framework;
using UIM.core.Geometry;

namespace UIM.Tests;

public class Circle2DTests
{
    private const double Tolerance = 1e-9;

    [Test]
    public void OperatorModeConstructsCircleFromThreePoints()
    {
        var first = new Point2D(7, -1);
        var second = new Point2D(2, 4);
        var third = new Point2D(-3, -1);

        Circle2D circle = Circle2D.CreateOperator(first, second, third);

        Assert.Multiple(() =>
        {
            Assert.That(circle.CenterX, Is.EqualTo(2).Within(Tolerance));
            Assert.That(circle.CenterY, Is.EqualTo(-1).Within(Tolerance));
            Assert.That(circle.Radius, Is.EqualTo(5).Within(Tolerance));
            Assert.That(circle.SignedDistanceTo(first), Is.EqualTo(0).Within(Tolerance));
            Assert.That(circle.SignedDistanceTo(second), Is.EqualTo(0).Within(Tolerance));
            Assert.That(circle.SignedDistanceTo(third), Is.EqualTo(0).Within(Tolerance));
        });
    }

    [Test]
    public void OperatorModeRejectsCollinearPoints()
    {
        Assert.That(
            () => Circle2D.CreateOperator(
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)),
            Throws.ArgumentException);
    }

    [Test]
    public void OperatorModeRejectsCoincidentPoints()
    {
        Assert.That(
            () => Circle2D.CreateOperator(
                new Point2D(0, 0),
                new Point2D(0, 0),
                new Point2D(1, 1)),
            Throws.ArgumentException);
    }

    [Test]
    public void MetrologistModeFitsExactCircleAndCalculatesStatistics()
    {
        double diagonal = 5 / Math.Sqrt(2);
        Point2D[] points =
        {
            new(8, -2),
            new(-2, -2),
            new(3, 3),
            new(3, -7),
            new(3 + diagonal, -2 + diagonal),
            new(3 - diagonal, -2 - diagonal)
        };

        CircleFitResult result = Circle2D.CreateMetrologist(points);

        Assert.Multiple(() =>
        {
            Assert.That(result.Circle.CenterX, Is.EqualTo(3).Within(Tolerance));
            Assert.That(result.Circle.CenterY, Is.EqualTo(-2).Within(Tolerance));
            Assert.That(result.Circle.Radius, Is.EqualTo(5).Within(Tolerance));
            Assert.That(result.Residuals, Has.All.EqualTo(0).Within(Tolerance));
            Assert.That(result.DegreesOfFreedom, Is.EqualTo(points.Length - 3));
            Assert.That(result.WeightedSquaredResiduals, Is.EqualTo(0).Within(Tolerance));
            Assert.That(result.StandardDeviation, Is.EqualTo(0).Within(Tolerance));
            Assert.That(result.Converged, Is.True);
        });
    }

    [Test]
    public void MetrologistModeUsesRadialResiduals()
    {
        Point2D[] points =
        {
            new(11, 0),
            new(-11, 0),
            new(0, 9),
            new(0, -9)
        };

        CircleFitResult result = Circle2D.CreateMetrologist(points);

        Assert.Multiple(() =>
        {
            Assert.That(result.Circle.CenterX, Is.EqualTo(0).Within(Tolerance));
            Assert.That(result.Circle.CenterY, Is.EqualTo(0).Within(Tolerance));
            Assert.That(result.Circle.Radius, Is.EqualTo(10).Within(Tolerance));
            Assert.That(result.Residuals, Is.EqualTo(new[] { 1.0, 1.0, -1.0, -1.0 })
                .Within(Tolerance));
            Assert.That(result.DegreesOfFreedom, Is.EqualTo(1));
            Assert.That(result.WeightedSquaredResiduals, Is.EqualTo(4).Within(Tolerance));
            Assert.That(result.StandardDeviation, Is.EqualTo(2).Within(Tolerance));
        });
    }

    [Test]
    public void MetrologistModeSupportsZeroWeightExclusion()
    {
        Point2D[] points =
        {
            new(5, 0),
            new(-5, 0),
            new(0, 5),
            new(0, -5),
            new(20, 0)
        };
        double[] weights = { 1, 1, 1, 1, 0 };

        CircleFitResult result = Circle2D.CreateMetrologist(points, weights);

        Assert.Multiple(() =>
        {
            Assert.That(result.Circle.CenterX, Is.EqualTo(0).Within(Tolerance));
            Assert.That(result.Circle.CenterY, Is.EqualTo(0).Within(Tolerance));
            Assert.That(result.Circle.Radius, Is.EqualTo(5).Within(Tolerance));
            Assert.That(result.Residuals[4], Is.EqualTo(15).Within(Tolerance));
            Assert.That(result.ActivePointCount, Is.EqualTo(4));
            Assert.That(result.DegreesOfFreedom, Is.EqualTo(1));
            Assert.That(result.WeightedSquaredResiduals, Is.EqualTo(0).Within(Tolerance));
        });
    }

    [Test]
    public void MetrologistModeRejectsTooFewPoints()
    {
        Point2D[] points =
        {
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        };

        Assert.That(
            () => Circle2D.CreateMetrologist(points),
            Throws.ArgumentException);
    }

    [Test]
    public void MetrologistModeRejectsInvalidWeights()
    {
        Point2D[] points =
        {
            new(1, 0),
            new(0, 1),
            new(-1, 0),
            new(0, -1)
        };

        Assert.That(
            () => Circle2D.CreateMetrologist(points, new[] { 1.0, 1.0, -1.0, 1.0 }),
            Throws.ArgumentException);
    }

    [Test]
    public void MetrologistModeRejectsCollinearPoints()
    {
        Point2D[] points =
        {
            new(0, 0),
            new(1, 1),
            new(2, 2),
            new(3, 3)
        };

        Assert.That(
            () => Circle2D.CreateMetrologist(points),
            Throws.ArgumentException);
    }
}
