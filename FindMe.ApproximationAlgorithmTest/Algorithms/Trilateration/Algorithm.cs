using FindMe.ApproximationAlgorithmTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Algorithms.Trilateration
{
    public class Trilateration
    {
        public static Tuple<Result, Result> Calculate(IList<Sensor> sensors, int parameterMaxTrilaterationTriangleDifference2, int parameterMaxTrilaterationTriangleDifference3)
        {
            if (sensors.Count() != 3)
            {
                return null;
            }

            var result1 = CircleIntersection.Intersection.Calculate(sensors[0], sensors[1]);
            var result2 = CircleIntersection.Intersection.Calculate(sensors[0], sensors[2]);
            var result3 = CircleIntersection.Intersection.Calculate(sensors[2], sensors[1]);

            if (result1 != null && result2 != null && result3 != null)
            {
                var distanceA1B1 = Math.Pow(Math.Pow(result1.Item1.X - result2.Item1.X, 2) + Math.Pow(result1.Item1.Y - result2.Item1.Y, 2), 0.5);
                var distanceA1B2 = Math.Pow(Math.Pow(result1.Item1.X - result2.Item2.X, 2) + Math.Pow(result1.Item1.Y - result2.Item2.Y, 2), 0.5);
                var distanceA1C1 = Math.Pow(Math.Pow(result1.Item1.X - result3.Item1.X, 2) + Math.Pow(result1.Item1.Y - result3.Item1.Y, 2), 0.5);
                var distanceA1C2 = Math.Pow(Math.Pow(result1.Item1.X - result3.Item2.X, 2) + Math.Pow(result1.Item1.Y - result3.Item2.Y, 2), 0.5);
                var distanceA2B1 = Math.Pow(Math.Pow(result1.Item2.X - result2.Item1.X, 2) + Math.Pow(result1.Item2.Y - result2.Item1.Y, 2), 0.5);
                var distanceA2B2 = Math.Pow(Math.Pow(result1.Item2.X - result2.Item2.X, 2) + Math.Pow(result1.Item2.Y - result2.Item2.Y, 2), 0.5);
                var distanceA2C1 = Math.Pow(Math.Pow(result1.Item2.X - result3.Item1.X, 2) + Math.Pow(result1.Item2.Y - result3.Item1.Y, 2), 0.5);
                var distanceA2C2 = Math.Pow(Math.Pow(result1.Item2.X - result3.Item2.X, 2) + Math.Pow(result1.Item2.Y - result3.Item2.Y, 2), 0.5);
                var distanceB1C1 = Math.Pow(Math.Pow(result2.Item1.X - result3.Item1.X, 2) + Math.Pow(result2.Item1.Y - result3.Item1.Y, 2), 0.5);
                var distanceB1C2 = Math.Pow(Math.Pow(result2.Item1.X - result3.Item2.X, 2) + Math.Pow(result2.Item1.Y - result3.Item2.Y, 2), 0.5);
                var distanceB2C1 = Math.Pow(Math.Pow(result2.Item2.X - result3.Item1.X, 2) + Math.Pow(result2.Item2.Y - result3.Item1.Y, 2), 0.5);
                var distanceB2C2 = Math.Pow(Math.Pow(result2.Item2.X - result3.Item2.X, 2) + Math.Pow(result2.Item2.Y - result3.Item2.Y, 2), 0.5);

                var sumA1B1C1 = distanceA1B1 + distanceA1C1 + distanceB1C1;
                var sumA1B1C2 = distanceA1B1 + distanceA1C2 + distanceB1C2;
                var sumA1B2C1 = distanceA1B2 + distanceA1C1 + distanceB2C1;
                var sumA1B2C2 = distanceA1B2 + distanceA1C2 + distanceB2C2;
                var sumA2B1C1 = distanceA2B1 + distanceA2C1 + distanceB1C1;
                var sumA2B1C2 = distanceA2B1 + distanceA2C2 + distanceB1C2;
                var sumA2B2C1 = distanceA2B2 + distanceA2C1 + distanceB2C1;
                var sumA2B2C2 = distanceA2B2 + distanceA2C2 + distanceB2C2;

                var pointA1B1C1 = new Result() { X = (result1.Item1.X + result2.Item1.X + result3.Item1.X) / 3, Y = (result1.Item1.Y + result2.Item1.Y + result3.Item1.Y) / 3 };
                var pointA1B1C2 = new Result() { X = (result1.Item1.X + result2.Item1.X + result3.Item2.X) / 3, Y = (result1.Item1.Y + result2.Item1.Y + result3.Item2.Y) / 3 };
                var pointA1B2C1 = new Result() { X = (result1.Item1.X + result2.Item2.X + result3.Item1.X) / 3, Y = (result1.Item1.Y + result2.Item2.Y + result3.Item1.Y) / 3 };
                var pointA1B2C2 = new Result() { X = (result1.Item1.X + result2.Item2.X + result3.Item2.X) / 3, Y = (result1.Item1.Y + result2.Item2.Y + result3.Item2.Y) / 3 };
                var pointA2B1C1 = new Result() { X = (result1.Item2.X + result2.Item1.X + result3.Item1.X) / 3, Y = (result1.Item2.Y + result2.Item1.Y + result3.Item1.Y) / 3 };
                var pointA2B1C2 = new Result() { X = (result1.Item2.X + result2.Item1.X + result3.Item2.X) / 3, Y = (result1.Item2.Y + result2.Item1.Y + result3.Item2.Y) / 3 };
                var pointA2B2C1 = new Result() { X = (result1.Item2.X + result2.Item2.X + result3.Item1.X) / 3, Y = (result1.Item2.Y + result2.Item2.Y + result3.Item1.Y) / 3 };
                var pointA2B2C2 = new Result() { X = (result1.Item2.X + result2.Item2.X + result3.Item2.X) / 3, Y = (result1.Item2.Y + result2.Item2.Y + result3.Item2.Y) / 3 };

                var values = new double[] { sumA1B1C1,
                                     sumA1B1C2,
                                     sumA1B2C1,
                                     sumA1B2C2,
                                     sumA2B1C1,
                                     sumA2B1C2,
                                     sumA2B2C1,
                                     sumA2B2C2 };

                var minFirst = values.Min();
                var first = minFirst == sumA1B1C1 ? pointA1B1C1 : (
                    minFirst == sumA1B1C2 ? pointA1B1C2 : (
                    minFirst == sumA1B2C1 ? pointA1B2C1 : (
                    minFirst == sumA1B2C2 ? pointA1B2C2 : (
                    minFirst == sumA2B1C1 ? pointA2B1C1 : (
                    minFirst == sumA2B1C2 ? pointA2B1C2 : (
                    minFirst == sumA2B2C1 ? pointA2B2C1 :
                    pointA2B2C2))))));

                values = values.Where(v => v != minFirst).ToArray();
                var minSecond = values.Min();
                Result second = null;
                if (minSecond - minFirst <= parameterMaxTrilaterationTriangleDifference3)
                {
                    second = minSecond == sumA1B1C1 ? pointA1B1C1 : (
                    minSecond == sumA1B1C2 ? pointA1B1C2 : (
                    minSecond == sumA1B2C1 ? pointA1B2C1 : (
                    minSecond == sumA1B2C2 ? pointA1B2C2 : (
                    minSecond == sumA2B1C1 ? pointA2B1C1 : (
                    minSecond == sumA2B1C2 ? pointA2B1C2 : (
                    minSecond == sumA2B2C1 ? pointA2B2C1 :
                    pointA2B2C2))))));
                }

                return new Tuple<Result, Result>(first, second);
            }
            else if (result1 != null && result2 != null || result1 != null && result3 != null || result2 != null && result3 != null)
            {
                if (result1 == null)
                {
                    result1 = result3;
                }
                else if (result2 == null)
                {
                    result2 = result3;
                }

                var distanceA1B1 = Math.Pow(Math.Pow(result1.Item1.X - result2.Item1.X, 2) + Math.Pow(result1.Item1.Y - result2.Item1.Y, 2), 0.5);
                var distanceA1B2 = Math.Pow(Math.Pow(result1.Item1.X - result2.Item2.X, 2) + Math.Pow(result1.Item1.Y - result2.Item2.Y, 2), 0.5);
                var distanceA2B1 = Math.Pow(Math.Pow(result1.Item2.X - result2.Item1.X, 2) + Math.Pow(result1.Item2.Y - result2.Item1.Y, 2), 0.5);
                var distanceA2B2 = Math.Pow(Math.Pow(result1.Item2.X - result2.Item2.X, 2) + Math.Pow(result1.Item2.Y - result2.Item2.Y, 2), 0.5);

                var pointA1B1 = new Result() { X = (result1.Item1.X + result2.Item1.X) / 2, Y = (result1.Item1.Y + result2.Item1.Y) / 2 };
                var pointA1B2 = new Result() { X = (result1.Item1.X + result2.Item2.X) / 2, Y = (result1.Item1.Y + result2.Item2.Y) / 2 };
                var pointA2B1 = new Result() { X = (result1.Item2.X + result2.Item1.X) / 2, Y = (result1.Item2.Y + result2.Item1.Y) / 2 };
                var pointA2B2 = new Result() { X = (result1.Item2.X + result2.Item2.X) / 2, Y = (result1.Item2.Y + result2.Item2.Y) / 2 };

                var values = new double[] { distanceA1B1,
                                     distanceA1B2,
                                     distanceA2B1,
                                     distanceA2B2 };

                var minFirst = values.Min();
                var first = minFirst == distanceA1B1 ? pointA1B1 : (
                    minFirst == distanceA1B2 ? pointA1B2 : (
                    minFirst == distanceA2B1 ? pointA2B1 : pointA2B2));

                values = values.Where(v => v != minFirst).ToArray();
                var minSecond = values.Min();
                Result second = null;

                if (minSecond - minFirst <= parameterMaxTrilaterationTriangleDifference2)
                {
                    second = minSecond == distanceA1B1 ? pointA1B1 : (
                        minSecond == distanceA1B2 ? pointA1B2 : (
                        minSecond == distanceA2B1 ? pointA2B1 : pointA2B2));
                }

                return new Tuple<Result, Result>(first, second);
            }
            else
            {
                return null;
            }
        }

        static double Norm(Point p) // get the norm of a vector
        {
            return Math.Pow(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2), .5);
        }

        struct Point
        {
            public double X { get; set; }

            public double Y { get; set; }
        }
    }
}
