using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Algorithms
{
	class IsGroundedCheckerTest
	{
		public static void Test()
		{
			var modelFilePath = @"..\..\..\data\models\LA020_tgt.mdl";

			var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));

			var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
			mongoOplogWriter.WriteLogName("GreedyGravityAI_IsGrounded");
			var state = State.CreateInitial(matrix.R, mongoOplogWriter);
			mongoOplogWriter.WriteInitialState(state);

			var groundedChecker = new IsGroundedChecker(matrix);

			var vertexex = new List<Vector>();

			for (int y = 0; y < matrix.R; y++)
			for (int x = 0; x < matrix.R; x++)
			for (int z = 0; z < matrix.R; z++)
			{
				var vector = new Vector(x, y, z);
				if (matrix.IsVoid(vector))
				{
					continue;
				}

				vertexex.Add(vector);
			}

			var rand = new Random();
			vertexex = vertexex.OrderBy(_ => rand.NextDouble()).ToList();

			foreach (var vector in vertexex)
			{
				if (groundedChecker.CanRemove(vector))
				{
					groundedChecker.UpdateWithClear(vector);
					continue;
				}

				mongoOplogWriter.WriteFill(vector);
			}

			mongoOplogWriter.Save();

		}
	}
}
