using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Infrastructure.Models
{
    //    public class _Matrix
    //    {
    //        private readonly bool[,,] matrix;
    //        public int R => matrix.GetLength(0);
    //
    //        private int connectedComponentAmount;
    //        private HashSet<Vector> bridges;
    //
    //        public _Matrix(bool[,,] matrix)
    //        {
    //            if (matrix.GetLength(0) != matrix.GetLength(1) || matrix.GetLength(1) != matrix.GetLength(2))
    //                throw new Exception("Matrix should have equal demensions");
    //
    //            this.matrix = matrix;
    //        }
    //
    //        public _Matrix(int r)
    //        {
    //            matrix = new bool[r, r, r];
    //
    //            bridges = new HashSet<Vector>();
    //            connectedComponentAmount = 0;
    //        }
    //
    //        public _Matrix(IReadOnlyList<string> layers)
    //        {
    //            var r = layers.Count;
    //            matrix = new bool[r, r, r];
    //            for (var y = 0; y < layers.Count; y++)
    //            {
    //                var lines = layers[y].Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    //                for (var z = R - 1; z > -1; z--)
    //                {
    //                    for (var x = 0; x < R; x++)
    //                    {
    //                        matrix[x, y, z] = lines[R - z - 1][x] == '1';
    //                    }
    //                }
    //            }
    //        }
    //
    //        public bool IsFull(Vector v)
    //        {
    //            return matrix[v.X, v.Y, v.Z];
    //        }
    //
    //        public bool IsVoid(Vector v)
    //        {
    //            return !matrix[v.X, v.Y, v.Z];
    //        }
    //
    //        public IEnumerable<Vector> GetAdjacents(Vector v)
    //        {
    //            return v.GetAdjacents().Where(Contains);
    //        }
    //
    //        public IEnumerable<Vector> GetNears(Vector v)
    //        {
    //            return v.GetNears().Where(Contains);
    //        }
    //
    //        public bool Contains(Vector v)
    //        {
    //            return v.X >= 0 && v.X < R &&
    //                   v.Y >= 0 && v.Y < R &&
    //                   v.Z >= 0 && v.Z < R;
    //        }
    //    }


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

		private HashSet<Vector> grounded = new HashSet<Vector>();

        public bool IsGrounded()
        {
            for (var x = 0; x < R; x++)
            {
                for (var y = 0; y < R; y++)
                {
                    for (var z = 0; z < R; z++)
                    {
                        if (!IsGrounded(new Vector(x, y, z)))
                            return false;
                    }
                }
            }

            return true;
        }

        public bool IsGrounded(Vector v)
        {
            if (IsVoid(v))
                return true;
            
            return CheckIsGrounded(v);
        }

        public bool CheckIsGrounded(Vector point)
        {
            var used = new HashSet<Vector>();
            var queue = new Queue<Vector>();
            queue.Enqueue(point);

            while (queue.Count != 0)
            {
                var v = queue.Dequeue();

                used.Add(v);

                if (v.Y == 0)
                    return true;

                foreach (var adj in GetAdjacents(v))
                {
                    if (IsFull(adj) && !used.Contains(adj))
                        queue.Enqueue(adj);
                }
            }

            return false;
//
//            visited.Add(point);
//            if (point.Y == 0 || grounded.Contains(point))
//            {
//                return true;
//            }
//
//            foreach (var adj in GetAdjacents(point))
//            {
//                if (IsFull(adj) && !visited.Contains(adj) && CheckIsGrounded(adj, visited))
//                {
//                    grounded.Add(point);
//                    return true;
//                }
//            }
//
//            return false;
        }

        public void Fill(Vector v)
        {
            matrix[v.X, v.Y, v.Z] = true;
        }

        public void Clear(Vector v)
        {
            matrix[v.X, v.Y, v.Z] = false;
            grounded.Clear();
        }
    }
}