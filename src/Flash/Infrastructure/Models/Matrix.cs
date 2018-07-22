using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Algorithms;

namespace Flash.Infrastructure.Models
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

        public Matrix(string[] layers)
        {
            var r = layers.Length;
            matrix = new bool[r, r, r];
            for (var y = 0; y < layers.Length; y++)
            {
                var lines = layers[y].Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (var z = R - 1; z > -1; z--)
                {
                    for (var x = 0; x < R; x++)
                    {
                        matrix[x, y, z] = lines[R - z - 1][x] == '1';
                    }
                }
            }
        }

        public Matrix Clone()
        {
            return new Matrix(matrix);
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
		
	    public static bool Contains(int R, Vector v)
	    {
		    return v.X >= 0 && v.X < R &&
		           v.Y >= 0 && v.Y < R &&
		           v.Z >= 0 && v.Z < R;
	    }

        public void Fill(Vector v)
        {
            matrix[v.X, v.Y, v.Z] = true;
        }

        public void Clear(Vector v)
        {
            matrix[v.X, v.Y, v.Z] = false;
        }
    }
}