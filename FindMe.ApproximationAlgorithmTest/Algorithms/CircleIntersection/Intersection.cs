using FindMe.ApproximationAlgorithmTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Algorithms.CircleIntersection
{
    public class Intersection
    {
        public static Tuple<Result, Result> Calculate(Sensor sensor1, Sensor sensor2)
        {
            // Find the distance between the centers.
            double dx = sensor1.X - sensor2.X;
            double dy = sensor1.Y - sensor2.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            // See how many solutions there are.
            if (dist > sensor1.Distance + sensor2.Distance)
            {
                // No solutions, the circles are too far apart.
                return null;
            }
            else if (dist < Math.Abs(sensor1.Distance - sensor2.Distance))
            {
                // No solutions, one circle contains the other.
                return null;
            }
            else if ((dist == 0) && (sensor1.Distance == sensor2.Distance))
            {
                // No solutions, the circles coincide.
                return null;
            }
            else
            {
                // Find a and h.
                double a = (sensor1.Distance * sensor1.Distance -
                    sensor2.Distance * sensor2.Distance + dist * dist) / (2 * dist);
                double h = Math.Sqrt(sensor1.Distance * sensor1.Distance - a * a);

                // Find P2.
                double cx2 = sensor1.X + a * (sensor2.X - sensor1.X) / dist;
                double cy2 = sensor1.Y + a * (sensor2.Y - sensor1.Y) / dist;

                // Get the points P3.
                var result1 = new Result() { X = (cx2 + h * (sensor2.Y - sensor1.Y) / dist), Y = (cy2 - h * (sensor2.X - sensor1.X) / dist) };
                var result2 = new Result() { X = (cx2 - h * (sensor2.Y - sensor1.Y) / dist), Y = (cy2 + h * (sensor2.X - sensor1.X) / dist) };

                return new Tuple<Result, Result>(result1, result2);
            }
        }
    }
}
