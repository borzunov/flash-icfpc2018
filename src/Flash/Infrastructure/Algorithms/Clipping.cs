using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
    public static class Clipping
    {
        public static int Clip(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else
            if (value > max)
                value = max;
            return value;
        }

        public static int Clip(int value, int abs) => Clip(value, -abs, abs);

        public static Vector Clip(Vector v, int abs) =>
            new Vector(Clip(v.X, abs), Clip(v.Y, abs), Clip(v.Z, abs));
    }
}