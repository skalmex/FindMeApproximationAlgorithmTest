using FindMe.ApproximationAlgorithmTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Algorithms.Multilateration
{
    public class Algorithm
    {
        public static Result Calculate(IList<Sensor> sensors)
        {
            if (sensors.Count() != 4)
            {
                return null;
            }

            var results = new List<Tuple<Result, Result>>()
            {
                CalculateIntersection(new List<Sensor>() { sensors[0], sensors[1], sensors[2] }),
                CalculateIntersection(new List<Sensor>() { sensors[0], sensors[1], sensors[3] }),
                CalculateIntersection(new List<Sensor>() { sensors[0], sensors[2], sensors[3] }),
                CalculateIntersection(new List<Sensor>() { sensors[1], sensors[2], sensors[3] })
            };

            var resultLists = results.Where(r => r != null).Select(r => new List<Result>() { r.Item1, r.Item2 }).ToList();

            if (resultLists.Count > 1)
            {
                double bestValue = double.MaxValue;
                List<Result> bestCombination = null;

                for (int i = 0; i < Convert.ToInt32(Math.Pow(resultLists.Count, 2)) - 1; i++)
                {
                    var listToCalculate = new List<Result>();

                    for (int j = 0; j < resultLists.Count; j++)
                    {
                        listToCalculate.Add(resultLists[j][(i >> j) % 2]);
                    }

                    var result = CalculateDistances(listToCalculate);

                    if (result < bestValue)
                    {
                        bestValue = result;
                        bestCombination = listToCalculate;
                    }
                }

                return new Result()
                {
                    X = bestCombination.Sum(x => x.X) / bestCombination.Count,
                    Y = bestCombination.Sum(x => x.Y) / bestCombination.Count,
                    Z = bestCombination.Sum(x => x.Z) / bestCombination.Count
                };
            }
            else
            {
                return null;
            }
        }

        public static double CalculateDistances(List<Result> results)
        {
            double result = 0;

            for (int i = 0; i < results.Count; i++)
            {
                for (int j = i + 1; j < results.Count; j++)
                {
                    result += Distance(results[i], results[j]);
                }
            }

            return result;
        }

        public static Tuple<Result, Result> CalculateIntersection(IList<Sensor> sensors)
        {
            if (sensors.Count() != 3)
            {
                return null;
            }

            var p1 = new Result() { X = sensors[0].X, Y = sensors[0].Y, Z = sensors[0].Z };
            var p2 = new Result() { X = sensors[1].X, Y = sensors[1].Y, Z = sensors[1].Z };
            var p3 = new Result() { X = sensors[2].X, Y = sensors[2].Y, Z = sensors[2].Z };

            var r1 = sensors[0].Distance;
            var r2 = sensors[1].Distance;
            var r3 = sensors[2].Distance;

            var temp1 = Substract(p2, p1);
            var ex = Divide(temp1, Normalize(temp1));
            var temp2 = Substract(p3, p1);
            var i = Dot(ex, temp2);
            var temp3 = Substract(temp2, Multiply(ex, i));
            var ey = Divide(temp3, Normalize(temp3));
            var ez = Cross(ex, ey);
            var d = Normalize(Substract(p2, p1));
            var j = Dot(ey, temp2);
            var x = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            var y = (r1 * r1 - r3 * r3 - 2 * i * x + i * i + j * j) / (2 * j);
            var temp4 = r1 * r1 - x * x - y * y;

            if (temp4 < 0)
            {
                return null;
            }

            var z = Math.Sqrt(temp4);

            var result1 = Add(Add(Add(p1, Multiply(ex, x)), Multiply(ey, y)), Multiply(ez, z));
            var result2 = Substract(Add(Add(p1, Multiply(ex, x)), Multiply(ey, y)), Multiply(ez, z));


            return new Tuple<Result, Result>(result1, result2);
        }

        public static double Distance(Result a, Result b)
        {
            var deltaX = b.X - a.X;
            var deltaY = b.Y - a.Y;
            var deltaZ = b.Z - a.Z;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        public static Result Add(Result a, Result b)
        {
            return new Result()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
        }

        public static Result Substract(Result a, Result b)
        {
            return new Result()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
        }

        public static Result Divide(Result a, double d)
        {
            if (d == 0f)
            {
                return new Result() { X = 0, Y = 0, Z = 0 };
            }
            else
            {
                return new Result()
                {
                    X = a.X / d,
                    Y = a.Y / d,
                    Z = a.Z / d
                };
            }
        }

        public static Result Multiply(Result a, double m)
        {
            return new Result()
            {
                X = a.X * m,
                Y = a.Y * m,
                Z = a.Z * m
            };
        }

        public static Result Cross(Result v1, Result v2)
        {
            var x = v1.Y * v2.Z - v2.Y * v1.Z;
            var y = (v1.X * v2.Z - v2.X * v1.Z) * -1;
            var z = v1.X * v2.Y - v2.X * v1.Y;

            return new Result() { X = x, Y = y, Z = z };
        }

        public static double Normalize(Result a)
        {
            return Math.Pow(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2) + Math.Pow(a.Z, 2), .5);
        }

        public static double Dot(Result a, Result b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
    }
}
