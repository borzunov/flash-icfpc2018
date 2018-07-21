using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Infrastructure.Models
{
	public class Matrix
	{
		private readonly HashSet<Vector> pointsAtGround = new HashSet<Vector>();
		private readonly HashSet<Vector> pointsNotInGround = new HashSet<Vector>();
		private readonly HashSet<Vector> groundedPoints = new HashSet<Vector>();
		private bool flag = false;

		public int R { get; }

		public Matrix(int r)
		{
			R = r;
		}

		public static Matrix Create(string[] layers)
		{
			var r = layers.Length;
			var matrix = new bool[r, r, r];
			for (var y = 0; y < layers.Length; y++)
			{
				var lines = layers[y].Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
				for (var z = r - 1; z > -1; z--)
				{
					for (var x = 0; x < r; x++)
					{
						matrix[x, y, z] = lines[r - z - 1][x] == '1';
					}
				}
			}

			return new Matrix(matrix);
		}

		public Matrix(bool[,,] matrix)
		{
			R = matrix.GetLength(0);

			if (matrix.GetLength(1) != R || matrix.GetLength(2) != R)
				throw new ArgumentException("Matrix should have equals size in width, height and length");

			for (var x = 0; x < R; x++)
			{
				for (var y = 0; y < R; y++)
				{
					for (var z = 0; z < R; z++)
					{
						var v = new Vector(x, y, z);

						if (matrix[x, y, z])
							Fill(v);
					}
				}
			}

			FullWellFormedRecomputing();
		}

		public bool IsFull(Vector v)
		{
			return pointsAtGround.Contains(v) || pointsNotInGround.Contains(v);
		}

		public bool IsVoid(Vector v)
		{
			return !IsFull(v);
		}

		public bool Contains(Vector v)
		{
			return v.X >= 0 && v.X < R &&
				   v.Y >= 0 && v.Y < R &&
				   v.Z >= 0 && v.Z < R;
		}

		public IEnumerable<Vector> GetAdjacents(Vector v)
		{
			return v.GetAdjacents().Where(Contains);
		}

		public IEnumerable<Vector> GetNears(Vector v)
		{
			return v.GetNears().Where(Contains);
		}

		public void Fill(Vector v)
		{
			flag = true;

			if (v.Y == 0)
			{
				groundedPoints.Add(v);
				pointsAtGround.Add(v);
			}
			else
			{
				pointsNotInGround.Add(v);

				foreach (var adj in GetAdjacents(v))
				{
					if (groundedPoints.Contains(adj))
					{
						groundedPoints.Add(v);
						break;
					}
				}
			}
		}

		public void Clear(Vector v)
		{
			flag = true;

			if (v.Y == 0)
				pointsAtGround.Remove(v);
			else
				pointsNotInGround.Remove(v);

			groundedPoints.Remove(v);
		}

		public bool IsWellFormed()
		{
			if(flag)
				FullWellFormedRecomputing();

			return groundedPoints.Count == pointsAtGround.Count + pointsNotInGround.Count;
		}

		public bool IsGrounded(Vector v)
		{
			if (flag)
				FullWellFormedRecomputing();

			return groundedPoints.Contains(v);
		}

		public void FullWellFormedRecomputing()
		{
			if (!flag)
				return;

			groundedPoints.Clear();

			var queue = new Queue<Vector>();
			var used = new HashSet<Vector>();

			foreach (var point in pointsAtGround)
			{
				queue.Enqueue(point);
				used.Add(point);
			}

			while (queue.Count != 0)
			{
				var v = queue.Dequeue();

				groundedPoints.Add(v);

				foreach (var adj in GetAdjacents(v).Where(vector => IsFull(vector) && !used.Contains(vector)))
				{
					used.Add(adj);
					queue.Enqueue(adj);
				}
			}

			flag = false;
		}
	}
}