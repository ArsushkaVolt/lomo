namespace UIM.core.Geometry;

public sealed class Circle2D
{
    private const int MaximumFitIterations = 100;
    private const double FitTolerance = 1e-12;

    public double CenterX { get; }

    public double CenterY { get; }

    public double Radius { get; }

    private Circle2D(double centerX, double centerY, double radius)
    {
        if (!double.IsFinite(centerX) || !double.IsFinite(centerY))
        {
            throw new ArgumentException("Координаты центра окружности должны быть конечными числами.");
        }

        if (!double.IsFinite(radius) || radius <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(radius),
                "Радиус окружности должен быть конечным положительным числом.");
        }

        CenterX = centerX;
        CenterY = centerY;
        Radius = radius;
    }

    // Операторский режим
    public static Circle2D CreateOperator(
        Point2D first,
        Point2D second,
        Point2D third)
    {
        ValidatePoint(first, nameof(first));
        ValidatePoint(second, nameof(second));
        ValidatePoint(third, nameof(third));

        double secondX = second.X - first.X;
        double secondY = second.Y - first.Y;
        double thirdX = third.X - first.X;
        double thirdY = third.Y - first.Y;

        if (!double.IsFinite(secondX) || !double.IsFinite(secondY) ||
            !double.IsFinite(thirdX) || !double.IsFinite(thirdY))
        {
            throw new ArgumentException("Разности координат точек должны быть конечными числами.");
        }

        double secondLength = Hypotenuse(secondX, secondY);
        double thirdLength = Hypotenuse(thirdX, thirdY);

        if (secondLength == 0 || thirdLength == 0)
        {
            throw new ArgumentException(
                "Для построения окружности необходимы три различные точки.");
        }

        double secondNormalX = secondX / secondLength;
        double secondNormalY = secondY / secondLength;
        double thirdNormalX = thirdX / thirdLength;
        double thirdNormalY = thirdY / thirdLength;
        double determinant =
            secondNormalX * thirdNormalY - secondNormalY * thirdNormalX;

        if (Math.Abs(determinant) <= 1e-14)
        {
            throw new ArgumentException(
                "Невозможно построить единственную окружность по точкам, лежащим на одной прямой.");
        }

        double secondRightSide = secondLength / 2;
        double thirdRightSide = thirdLength / 2;
        double offsetX =
            (secondRightSide * thirdNormalY - secondNormalY * thirdRightSide) /
            determinant;
        double offsetY =
            (secondNormalX * thirdRightSide - secondRightSide * thirdNormalX) /
            determinant;
        double centerX = first.X + offsetX;
        double centerY = first.Y + offsetY;
        double radius = Hypotenuse(offsetX, offsetY);

        return new Circle2D(centerX, centerY, radius);
    }


    // Метрологический режим
    public static CircleFitResult CreateMetrologist(
        IReadOnlyList<Point2D> points,
        IReadOnlyList<double>? weights = null)
    {
        ArgumentNullException.ThrowIfNull(points);

        if (points.Count < 4)
        {
            throw new ArgumentException(
                "Для метрологического режима необходимо не менее четырёх точек.",
                nameof(points));
        }

        if (weights is not null && weights.Count != points.Count)
        {
            throw new ArgumentException(
                "Количество весов должно совпадать с количеством точек.",
                nameof(weights));
        }

        var effectiveWeights = new double[points.Count];
        double totalWeight = 0;
        double weightedX = 0;
        double weightedY = 0;
        int activePointCount = 0;

        for (int i = 0; i < points.Count; i++)
        {
            Point2D point = points[i];
            ValidatePoint(point, $"{nameof(points)}[{i}]");

            double weight = weights is null ? 1.0 : weights[i];

            if (!double.IsFinite(weight) || weight < 0)
            {
                throw new ArgumentException(
                    "Веса должны быть конечными неотрицательными числами.",
                    nameof(weights));
            }

            effectiveWeights[i] = weight;

            if (weight == 0)
            {
                continue;
            }

            totalWeight += weight;
            weightedX += weight * point.X;
            weightedY += weight * point.Y;
            activePointCount++;
        }

        if (activePointCount < 4 || totalWeight <= 0 || !double.IsFinite(totalWeight))
        {
            throw new ArgumentException(
                "Не менее четырёх точек должны иметь положительные веса.",
                nameof(weights));
        }

        double meanX = weightedX / totalWeight;
        double meanY = weightedY / totalWeight;

        if (!double.IsFinite(meanX) || !double.IsFinite(meanY))
        {
            throw new ArgumentException("Взвешенный центр точек должен иметь конечные координаты.", nameof(points));
        }

        var normalizedX = new double[points.Count];
        var normalizedY = new double[points.Count];
        double coordinateScale = 0;

        for (int i = 0; i < points.Count; i++)
        {
            double dx = points[i].X - meanX;
            double dy = points[i].Y - meanY;

            if (!double.IsFinite(dx) || !double.IsFinite(dy))
            {
                throw new ArgumentException(
                    "Разности координат точек должны быть конечными числами.",
                    nameof(points));
            }

            if (effectiveWeights[i] > 0)
            {
                coordinateScale = Math.Max(coordinateScale, Hypotenuse(dx, dy));
            }

            normalizedX[i] = dx;
            normalizedY[i] = dy;
        }

        if (coordinateScale == 0 || !double.IsFinite(coordinateScale))
        {
            throw new ArgumentException(
                "Точки не имеют достаточного разброса для определения окружности.",
                nameof(points));
        }

        for (int i = 0; i < points.Count; i++)
        {
            normalizedX[i] /= coordinateScale;
            normalizedY[i] /= coordinateScale;
        }

        (double centerX, double centerY, double radius) = CreateInitialEstimate(
            normalizedX,
            normalizedY,
            effectiveWeights,
            totalWeight);

        double currentCost = CalculateCost(
            normalizedX,
            normalizedY,
            effectiveWeights,
            centerX,
            centerY,
            radius);
        double damping = 1e-3;
        bool converged = false;
        int iterations = 0;

        for (int iteration = 0; iteration < MaximumFitIterations; iteration++)
        {
            iterations = iteration + 1;
            var normalMatrix = new double[3, 3];
            var gradient = new double[3];

            BuildNormalSystem(
                normalizedX,
                normalizedY,
                effectiveWeights,
                centerX,
                centerY,
                radius,
                normalMatrix,
                gradient);

            for (int i = 0; i < 3; i++)
            {
                normalMatrix[i, i] += damping * Math.Max(normalMatrix[i, i], 1.0);
                gradient[i] = -gradient[i];
            }

            if (!TrySolve3x3(normalMatrix, gradient, out double[] step))
            {
                break;
            }

            double stepLength = Hypotenuse(Hypotenuse(step[0], step[1]), step[2]);
            double parameterLength = Hypotenuse(Hypotenuse(centerX, centerY), radius);

            if (stepLength <= FitTolerance * (1 + parameterLength))
            {
                converged = true;
                break;
            }

            double candidateCenterX = centerX + step[0];
            double candidateCenterY = centerY + step[1];
            double candidateRadius = radius + step[2];

            if (!double.IsFinite(candidateCenterX) ||
                !double.IsFinite(candidateCenterY) ||
                !double.IsFinite(candidateRadius) ||
                candidateRadius <= 0)
            {
                damping *= 10;
                continue;
            }

            double candidateCost = CalculateCost(
                normalizedX,
                normalizedY,
                effectiveWeights,
                candidateCenterX,
                candidateCenterY,
                candidateRadius);

            if (candidateCost < currentCost)
            {
                double improvement = currentCost - candidateCost;
                centerX = candidateCenterX;
                centerY = candidateCenterY;
                radius = candidateRadius;
                currentCost = candidateCost;
                damping = Math.Max(damping * 0.3, 1e-15);

                if (improvement <= FitTolerance * (1 + currentCost))
                {
                    converged = true;
                    break;
                }
            }
            else
            {
                damping *= 10;

                if (!double.IsFinite(damping))
                {
                    break;
                }
            }
        }

        double actualCenterX = meanX + coordinateScale * centerX;
        double actualCenterY = meanY + coordinateScale * centerY;
        double actualRadius = coordinateScale * radius;
        var circle = new Circle2D(actualCenterX, actualCenterY, actualRadius);
        var residuals = new double[points.Count];
        double weightedSquaredResiduals = 0;

        for (int i = 0; i < points.Count; i++)
        {
            double residual = circle.SignedDistanceTo(points[i]);
            residuals[i] = residual;
            weightedSquaredResiduals += effectiveWeights[i] * residual * residual;
        }

        if (!double.IsFinite(weightedSquaredResiduals))
        {
            throw new InvalidOperationException("Аппроксимация окружности привела к некорректному результату.");
        }

        int degreesOfFreedom = activePointCount - 3;
        double standardDeviation = Math.Sqrt(weightedSquaredResiduals / degreesOfFreedom);

        return new CircleFitResult(
            circle,
            residuals,
            effectiveWeights,
            activePointCount,
            degreesOfFreedom,
            weightedSquaredResiduals,
            standardDeviation,
            converged,
            iterations);
    }

    public double SignedDistanceTo(Point2D point)
    {
        ValidatePoint(point, nameof(point));
        return Hypotenuse(point.X - CenterX, point.Y - CenterY) - Radius;
    }

    public double DistanceTo(Point2D point)
    {
        return Math.Abs(SignedDistanceTo(point));
    }

    private static (double CenterX, double CenterY, double Radius) CreateInitialEstimate(
        double[] x,
        double[] y,
        double[] weights,
        double totalWeight)
    {
        var normalMatrix = new double[3, 3];
        var rightSide = new double[3];

        for (int index = 0; index < x.Length; index++)
        {
            double weight = weights[index];

            if (weight == 0)
            {
                continue;
            }

            double squaredRadius = x[index] * x[index] + y[index] * y[index];
            double[] row = { x[index], y[index], 1.0 };

            for (int i = 0; i < 3; i++)
            {
                rightSide[i] -= weight * row[i] * squaredRadius;

                for (int j = 0; j < 3; j++)
                {
                    normalMatrix[i, j] += weight * row[i] * row[j];
                }
            }
        }

        if (!TrySolve3x3(normalMatrix, rightSide, out double[] parameters))
        {
            throw new ArgumentException(
                "Невозможно аппроксимировать единственную окружность по заданному распределению точек.");
        }

        double centerX = -parameters[0] / 2;
        double centerY = -parameters[1] / 2;
        double weightedRadius = 0;

        for (int i = 0; i < x.Length; i++)
        {
            if (weights[i] > 0)
            {
                weightedRadius += weights[i] *
                    Hypotenuse(x[i] - centerX, y[i] - centerY);
            }
        }

        double radius = weightedRadius / totalWeight;

        if (!double.IsFinite(centerX) || !double.IsFinite(centerY) ||
            !double.IsFinite(radius) || radius <= 0)
        {
            throw new ArgumentException("Начальная оценка окружности содержит некорректные значения.");
        }

        return (centerX, centerY, radius);
    }

    private static double CalculateCost(
        double[] x,
        double[] y,
        double[] weights,
        double centerX,
        double centerY,
        double radius)
    {
        double cost = 0;

        for (int i = 0; i < x.Length; i++)
        {
            if (weights[i] == 0)
            {
                continue;
            }

            double residual = Hypotenuse(x[i] - centerX, y[i] - centerY) - radius;
            cost += weights[i] * residual * residual;
        }

        return cost;
    }

    private static void BuildNormalSystem(
        double[] x,
        double[] y,
        double[] weights,
        double centerX,
        double centerY,
        double radius,
        double[,] normalMatrix,
        double[] gradient)
    {
        for (int index = 0; index < x.Length; index++)
        {
            double weight = weights[index];

            if (weight == 0)
            {
                continue;
            }

            double dx = centerX - x[index];
            double dy = centerY - y[index];
            double distance = Hypotenuse(dx, dy);
            double residual = distance - radius;
            double derivativeX = distance == 0 ? 0 : dx / distance;
            double derivativeY = distance == 0 ? 0 : dy / distance;
            double[] jacobian = { derivativeX, derivativeY, -1.0 };

            for (int i = 0; i < 3; i++)
            {
                gradient[i] += weight * jacobian[i] * residual;

                for (int j = 0; j < 3; j++)
                {
                    normalMatrix[i, j] += weight * jacobian[i] * jacobian[j];
                }
            }
        }
    }

    private static bool TrySolve3x3(
        double[,] coefficients,
        double[] rightSide,
        out double[] solution)
    {
        var augmented = new double[3, 4];
        double scale = 0;

        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                augmented[row, column] = coefficients[row, column];
                scale = Math.Max(scale, Math.Abs(coefficients[row, column]));
            }

            augmented[row, 3] = rightSide[row];
        }

        if (scale == 0 || !double.IsFinite(scale))
        {
            solution = Array.Empty<double>();
            return false;
        }

        for (int column = 0; column < 3; column++)
        {
            int pivotRow = column;

            for (int row = column + 1; row < 3; row++)
            {
                if (Math.Abs(augmented[row, column]) >
                    Math.Abs(augmented[pivotRow, column]))
                {
                    pivotRow = row;
                }
            }

            if (Math.Abs(augmented[pivotRow, column]) <= scale * 1e-14)
            {
                solution = Array.Empty<double>();
                return false;
            }

            if (pivotRow != column)
            {
                for (int index = column; index < 4; index++)
                {
                    (augmented[column, index], augmented[pivotRow, index]) =
                        (augmented[pivotRow, index], augmented[column, index]);
                }
            }

            for (int row = column + 1; row < 3; row++)
            {
                double factor = augmented[row, column] / augmented[column, column];

                for (int index = column; index < 4; index++)
                {
                    augmented[row, index] -= factor * augmented[column, index];
                }
            }
        }

        solution = new double[3];

        for (int row = 2; row >= 0; row--)
        {
            double value = augmented[row, 3];

            for (int column = row + 1; column < 3; column++)
            {
                value -= augmented[row, column] * solution[column];
            }

            solution[row] = value / augmented[row, row];

            if (!double.IsFinite(solution[row]))
            {
                solution = Array.Empty<double>();
                return false;
            }
        }

        return true;
    }

    private static void ValidatePoint(Point2D? point, string parameterName)
    {
        if (point is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (!double.IsFinite(point.X) || !double.IsFinite(point.Y))
        {
            throw new ArgumentException("Координаты точки должны быть конечными числами.", parameterName);
        }
    }

    private static double Hypotenuse(double first, double second)
    {
        first = Math.Abs(first);
        second = Math.Abs(second);
        double maximum = Math.Max(first, second);

        if (maximum == 0)
        {
            return 0;
        }

        double scaledFirst = first / maximum;
        double scaledSecond = second / maximum;

        return maximum * Math.Sqrt(
            scaledFirst * scaledFirst + scaledSecond * scaledSecond);
    }
}

public sealed class CircleFitResult
{
    internal CircleFitResult(
        Circle2D circle,
        double[] residuals,
        double[] weights,
        int activePointCount,
        int degreesOfFreedom,
        double weightedSquaredResiduals,
        double standardDeviation,
        bool converged,
        int iterations)
    {
        Circle = circle;
        Residuals = Array.AsReadOnly(residuals);
        Weights = Array.AsReadOnly(weights);
        ActivePointCount = activePointCount;
        DegreesOfFreedom = degreesOfFreedom;
        WeightedSquaredResiduals = weightedSquaredResiduals;
        StandardDeviation = standardDeviation;
        Converged = converged;
        Iterations = iterations;
    }

    public Circle2D Circle { get; }

    public IReadOnlyList<double> Residuals { get; }

    public IReadOnlyList<double> Weights { get; }

    public int ActivePointCount { get; }

    public int DegreesOfFreedom { get; }

    public double WeightedSquaredResiduals { get; }

    public double StandardDeviation { get; }

    public bool Converged { get; }

    public int Iterations { get; }
}