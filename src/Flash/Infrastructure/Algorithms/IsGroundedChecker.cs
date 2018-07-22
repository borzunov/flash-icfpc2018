using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	public class IsGroundedChecker
	{
		private DynamicConnectivity.DynamicConnectivity ConnectivityChecker;
		private Dictionary<Vector, int> Encoder = new Dictionary<Vector, int>();
		private Dictionary<int, Vector> Decoder = new Dictionary<int, Vector>();
		private Matrix Matrix;
		private int UnderGroundCode;

		private int UpdateEncoder(Vector vector)
		{
			var code = Encoder.Count;
			Encoder[vector] = code;
			Decoder[code] = vector;
			return code;
		}

		private Vector Decode(int code)
		{
			return Decoder[code];
		}

		private int Encode(Vector vector)
		{
			if (Encoder.TryGetValue(vector, out var code))
				return code;
			return UpdateEncoder(vector);
		}

		//На вход должна подаваться только приземленная матрица
		public IsGroundedChecker(Matrix matrix)
		{
			ConnectivityChecker = new DynamicConnectivity.DynamicConnectivity(matrix.R*matrix.R*matrix.R+1);

			UnderGroundCode = UpdateEncoder(new Vector(-1, -1, -1));

			for (int y = 0; y < matrix.R; y++)
			for (int x = 0; x < matrix.R; x++)
			for (int z = 0; z < matrix.R; z++)
			{
				var vector = new Vector(x,y,z);
				if(matrix.IsVoid(vector))
					continue;
				var srcCode = Encode(vector);

				foreach (var adj in vector.GetAdjacents()
					.Where(matrix.Contains)
					.Where(matrix.IsFull)
					.Where(adj => ComponentsSum(adj-vector) > 0))
				{
					var dstCode = Encode(adj);
					ConnectivityChecker.Link(srcCode, dstCode);
				}

				if (vector.Y == 0)
				{
					ConnectivityChecker.Link(srcCode, UnderGroundCode);
				}
			}

			Matrix = matrix.Clone();
			
			for (int y = 0; y < matrix.R; y++)
			for (int x = 0; x < matrix.R; x++)
			for (int z = 0; z < matrix.R; z++)
			{

				var vector = new Vector(x, y, z);
				if(Matrix.IsVoid(vector))
					continue;
				
				if(!ConnectivityChecker.IsConnected(Encode(vector), UnderGroundCode))
					throw new Exception("matrix is not grounded");
			}
		}

		private void LinkWithAdjancents(Vector vector)
		{
			var code = Encode(vector);
			foreach (var adj in vector.GetAdjacents()
				.Where(Matrix.Contains)
				.Where(Matrix.IsFull))
			{
				var dstCode = Encode(adj);
				ConnectivityChecker.Link(code, dstCode);
			}

			if (vector.Y == 0)
			{
				ConnectivityChecker.Link(code, UnderGroundCode);
			}
		}

		public void UpdateWithFill(Vector vector, bool lowHarmonic = true)
		{
			if(Matrix.IsFull(vector))
				return;

			Matrix.Fill(vector);
			var code = Encode(vector);

			LinkWithAdjancents(vector);

			if (!ConnectivityChecker.IsConnected(code, UnderGroundCode) && lowHarmonic)
			{
				throw new Exception($"Вектор {vector} не заземлен. Используйте CanFill!");
			}
		}

		public bool CanPlace(Vector vector)
		{
			if (Matrix.IsFull(vector))
				return true;

			return vector.GetAdjacents()
				.Where(Matrix.Contains)
				.Where(Matrix.IsFull)
				.Any();
		}

		public bool CanPlace(IList<Vector> vectors)
		{
			if (vectors.Count == 0)
				return true;

			foreach (var vector in vectors.Where(vector => Matrix.IsVoid(vector)))
			{
				var code = Encode(vector);
				var adjsCodes = vector.GetAdjacents()
					.Where(Matrix.Contains)
					.Where(Matrix.IsFull)
					.Select(Encode);

				foreach (var dstCode in adjsCodes)
				{
					ConnectivityChecker.Link(code, dstCode);
				}
			}

			var canPlace = vectors.All(vector => ConnectivityChecker.IsConnected(Encode(vector), UnderGroundCode));

			foreach (var vector in vectors.Where(vector => Matrix.IsVoid(vector)))
			{
				var code = Encode(vector);

				var adjsCodes = vector.GetAdjacents()
					.Where(Matrix.Contains)
					.Where(Matrix.IsFull)
					.Select(Encode);

				foreach (var dstCode in adjsCodes)
				{
					ConnectivityChecker.Cut(code, dstCode);
				}
			}

			return canPlace;
		}

		public bool CanRemove(Vector vector, bool lowHarmonic = true)
		{
			if (Matrix.IsVoid(vector))
				return true;

			var code = Encode(vector);
			var adjsCodes = vector.GetAdjacents()
				.Where(Matrix.Contains)
				.Where(Matrix.IsFull)
				.Select(Encode)
				.ToList();

			bool canRemove = true;
			foreach (var dstCode in adjsCodes)
			{
				ConnectivityChecker.Cut(code, dstCode);
				canRemove &= ConnectivityChecker.IsConnected(dstCode, UnderGroundCode);
			}

			foreach (var dstCode in adjsCodes)
			{
				ConnectivityChecker.Link(code, dstCode);
			}

			return canRemove || !lowHarmonic;
		}

		public bool CanRemove(ICollection<Vector> vectors)
		{
			bool canRemove = true;
			foreach (var vector in vectors.Where(vector => Matrix.IsFull(vector)))
			{
				var code = Encode(vector);
				var adjsCodes = vector.GetAdjacents()
					.Where(Matrix.Contains)
					.Where(Matrix.IsFull)
					.Select(Encode);

				foreach (var dstCode in adjsCodes)
				{
					ConnectivityChecker.Cut(code, dstCode);
					canRemove &= ConnectivityChecker.IsConnected(dstCode, UnderGroundCode);
				}
			}

			foreach(var vector in vectors.Where(vector => Matrix.IsFull(vector)))
			{
				var code = Encode(vector);
				
				var adjsCodes = vector.GetAdjacents()
					.Where(Matrix.Contains)
					.Where(Matrix.IsFull)
					.Select(Encode);

				foreach (var dstCode in adjsCodes)
				{
					ConnectivityChecker.Link(code, dstCode);
				}
			}

			return canRemove;
		}

		public void UpdateWithClear(Vector vector, bool lowHarmonic = true)
		{
			if (Matrix.IsVoid(vector))
				return;

			Matrix.Clear(vector);
			var code = Encode(vector);
			
			foreach (var adj in vector.GetAdjacents()
				.Where(Matrix.Contains)
				.Where(Matrix.IsFull))
			{
				var dstCode = Encode(adj);
				ConnectivityChecker.Cut(code, dstCode);
				if(!ConnectivityChecker.IsConnected(UnderGroundCode, dstCode) && lowHarmonic)
					throw new Exception($"Вектор {vector} - мост. Вершина {adj} полетела вниз. Используйте CanClear!");

			}

			if (vector.Y == 0)
			{
				ConnectivityChecker.Cut(code, UnderGroundCode);
			}
		}




		private int ComponentsSum(Vector vector)
		{
			return vector.X + vector.Y + vector.Z;
		}
	}
}
