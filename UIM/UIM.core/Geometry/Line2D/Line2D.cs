namespace UIM.core.Geometry;

public class Line2D
{
    public double A { get; }
    public double B { get; }

    public Line2D(double a, double b)
    {
        A = a;
        B = b;
    }

    public double GetY(double x)
    {
        return A * x + B;
    }
}