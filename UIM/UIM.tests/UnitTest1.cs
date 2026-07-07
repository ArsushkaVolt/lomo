using NUnit.Framework;
using UIM.core.Geometry;

namespace UIM.Tests;

public class PointTests
{
    [Test]
    public void PointCreateTest()
    {
        var point = new Point2D(5, 10);

        Assert.That(point.X, Is.EqualTo(5));
        Assert.That(point.Y, Is.EqualTo(10));
    }

    [Test]
    public void DistanceBetweenPointsTest()
    {
        var p1 = new Point2D(0, 0);
        var p2 = new Point2D(3, 4);

        double distance = p1.DistanceTo(p2);

        Assert.That(distance, Is.EqualTo(5));
    }
}