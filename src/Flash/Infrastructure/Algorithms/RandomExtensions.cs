using Flash.Infrastructure.Models;
using System;

namespace Flash.Infrastructure.Algorithms
{
    public static class RandomExtensions
    {
        public static double NextNormal(this Random rand, double mean, double sigma)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double stdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);
            return mean + sigma * stdNormal;
        }
    }
}
