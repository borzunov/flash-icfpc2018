using System;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
    public class PointCounter
    {
        public readonly int R;
        private readonly int[,,] sums;

        public PointCounter(Matrix matrix)
        {
            R = matrix.R;

            sums = new int[R + 1, R + 1, R + 1];
            Update(matrix, new Vector(0, 0, 0));
        }

        public void Update(Matrix matrix, Vector minChanged)
        {
            if (R != matrix.R)
                throw new ArgumentException("`matrix` has wrong size");

            var content = matrix.GetContent();
            for (var i = minChanged.X + 1; i <= R; i++)
            for (var j = minChanged.Y + 1; j <= R; j++)
            for (var k = minChanged.Z + 1; k <= R; k++)
                sums[i, j, k] = sums[i - 1, j, k] + sums[i, j - 1, k] + sums[i, j, k - 1]
                    - sums[i - 1, j - 1, k] - sums[i - 1, j, k - 1] - sums[i, j - 1, k - 1]
                    + sums[i - 1, j - 1, k - 1]
                    + (content[i - 1, j - 1, k - 1] ? 1 : 0);
        }

        public int CountFulls(Region region)
        {
            var a = region.Min;
            var b = region.Max + new Vector(1, 1, 1);
            return sums[b.X, b.Y, b.Z]
                - sums[a.X, b.Y, b.Z] - sums[b.X, a.Y, b.Z] - sums[b.X, b.Y, a.Z]
                + sums[a.X, a.Y, b.Z] + sums[a.X, b.Y, a.Z] + sums[b.X, a.Y, a.Z]
                - sums[a.X, a.Y, a.Z];
        }

        public int TotalFulls => sums[R, R, R];
    }
}
