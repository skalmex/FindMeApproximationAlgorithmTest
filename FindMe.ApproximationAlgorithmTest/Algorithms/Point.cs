using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Algorithms
{
    public class Point
    {
        public double X { get; private set; }

        public double Y { get; private set; }

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(decimal x, decimal y)
        {
            this.X = Convert.ToDouble(x);
            this.Y = Convert.ToDouble(y);
        }

        public double GetDistanceTo(Point dest)
        {
            return Math.Sqrt(Math.Pow((dest.X - this.X), 2) + Math.Pow((dest.Y - this.Y), 2));
        }
    }
}
