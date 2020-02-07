using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Algorithms.LinearRegression
{
    public class Algorithm
    {
        public static Point[] Calculate(Point[] points)
        {
            if (points.Length > 1)
            {
                Tuple<double, double> lineParams = Fit.Line(points.Select(p => Math.Round(p.X, 4)).ToArray(), points.Select(p => Math.Round(p.Y, 4)).ToArray());
                double a = Math.Round(lineParams.Item1, 4); //intercept
                double b = Math.Round(lineParams.Item2, 4); //slope

                return points.Select(p => new Point(p.X, a + b * p.X)).ToArray();
            }
            else
                return points;
        }
    }
}
