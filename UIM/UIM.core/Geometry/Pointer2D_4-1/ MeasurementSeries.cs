

namespace UIM.core.Geometry.Point2D_4_1;



public  class MeasurementSeries
{
    // список дл€ последовательность сохр точек n раз 
    private readonly List<Point2D> points = new();


    // ƒобавлени€ нового измерени€ 
    public void AddMeasurement(Point2D point)
    {
        points.Add(point);
    }

    // –асчет ср значени€ координат 
    public Point2D GetAverage()
    {
        if(points.Count == 0)
        {
            return new Point2D(0, 0);

        }

        double X = points.Average( p => p.X );
        double Y = points.Average(p => p.Y);

        return new Point2D(X, Y);
    }

    // –асчет ср знач координат 

    public (double SkoX, double SkoY) GetSko()
    {
        int n = points.Count;

        if(n <= 1)
        {
            return(0.0,0.0);
        }
        Point2D avg = GetAverage();

        double SumX = points.Sum(p => Math.Pow(p.X - avg.X, 2));
        double SumY = points.Sum(p => Math.Pow(p.Y - avg.Y, 2));

        double skoX = Math.Sqrt(SumX / (n - 1));
        double skoY = Math.Sqrt(SumY / (n - 1));

        return (skoX, skoY);


    }

    public void Clear()
    {
        points.Clear();
    }
}


