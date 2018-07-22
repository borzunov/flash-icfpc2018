using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
    public class PointCounter
    {
        private readonly int[,,] sums;

        public PointCounter(Matrix matrix)
        {
            sums = new int[matrix.R + 1, matrix.R + 1, matrix.R + 1];
            for (var i = 1; i <= matrix.R; i++)
                for (var j = 1; j <= matrix.R; j++)
                    for (var k = 1; k <= matrix.R; k++)
                        sums[i, j, k] = sums[i - 1, j, k] + sums[i, j - 1, k] + sums[i, j, k - 1]
                            - sums[i - 1, j - 1, k] - sums[i - 1, j, k - 1] - sums[i, j - 1, k - 1]
                            + sums[i - 1, j - 1, k - 1]
                            + (matrix.IsFull(new Vector(i, j, k)) ? 1 : 0);
        }

        public int CountPoints(Vector a, Vector b)
            // Use segments: [a.X; b.X] x [a.Y; b.Y] x [a.Z; b.Z]
        {
            b = b + new Vector(1, 1, 1);
            return sums[b.X, b.Y, b.Z]
                - sums[a.X, b.Y, b.Z] - sums[b.X, a.Y, b.Z] - sums[b.X, b.Y, a.Z]
                + sums[a.X, a.Y, b.Z] + sums[a.X, b.Y, a.Z] + sums[b.X, a.Y, a.Z]
                - sums[a.X, a.Y, a.Z];
        }
    }
}
