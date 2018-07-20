using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Infrastructure
{
    public class Matrix
    {
        private readonly bool[,,] matrix;

        public int R => matrix.GetLength(0);

        public Matrix(bool[,,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1) || matrix.GetLength(1) != matrix.GetLength(2))
            {
                throw new Exception("Matrix should have equal demensions");
            }

            this.matrix = matrix;
        }

        public Matrix(int r)
        {
            matrix = new bool[r, r, r];
        }

        public bool IsFull(Vector v)
        {
            return matrix[v.X, v.Y, v.Z];
        }

        public bool IsVoid(Vector v)
        {
            return !matrix[v.X, v.Y, v.Z];
        }

        public IEnumerable<Vector> GetAdjacents(Vector v)
        {
            return v.GetAdjacents().Where(Contains);
        }

        public IEnumerable<Vector> GetNears(Vector v)
        {
            return v.GetNears().Where(Contains);
        }

        public bool Contains(Vector v)
        {
            return v.X >= 0 && v.X < R &&
                   v.Y >= 0 && v.Y < R &&
                   v.Z >= 0 && v.Z < R;
        }

        private HashSet<Vector> grounded;

        public bool IsGrounded(Vector v)
        {
            if (v.Y == 0 || grounded.Contains(v))
            {
                return true;
            }

            foreach (var adj in GetAdjacents(v))
            {
                if (IsGrounded(adj))
                {
                    grounded.Add(v);
                    return true;
                }
            }

            return false;
        }
    }
}
