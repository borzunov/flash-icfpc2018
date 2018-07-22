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
            for (var i = 1; i <= R; i++)
                for (var j = 1; j <= R; j++)
                    for (var k = 1; k <= R; k++)
                        sums[i, j, k] = sums[i - 1, j, k] + sums[i, j - 1, k] + sums[i, j, k - 1]
                            - sums[i - 1, j - 1, k] - sums[i - 1, j, k - 1] - sums[i, j - 1, k - 1]
                            + sums[i - 1, j - 1, k - 1]
                            + (matrix.IsFull(new Vector(i - 1, j - 1, k - 1)) ? 1 : 0);
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
