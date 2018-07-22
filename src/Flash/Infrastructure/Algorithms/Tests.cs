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
	class Tests
	{
		public static void SearchTest()
		{

			//var trackFilePath = @"..\..\..\data\track\LA001.nbt";
			var modelFilePath = @"..\..\..\data\models\LA180_tgt.mdl";

			var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));
			//var ai = new GreedyGravityAI(matrix);

			Console.WriteLine("matrix loaded");

			var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
			mongoOplogWriter.WriteLogName("GreedyGravityAI_IsGrounded");
			var state = State.CreateInitial(matrix.R, mongoOplogWriter);
			mongoOplogWriter.WriteInitialState(state);

			var groundedChecker = new IsGroundedChecker(matrix);
			var startPosition = new Vector(0, 0, 0);

			for (int i = 0; i < matrix.R; i++)
			{
				for (int j = 0; j < matrix.R; j++)
				{
					for (int k = 0; k < matrix.R; k++)
					{
						var vector = new Vector(i, j, k);
						if (matrix.IsVoid(vector))
							continue;
						mongoOplogWriter.WriteColor(vector, "0000FF", 0.5);
					}
				}
			}


			Console.WriteLine("matrix inited");

			var rand = new Random(15);
			var forbidden = new HashSet<Vector>();
			Console.WriteLine("start");
			for (int i = 0; i < 1002; i++)
			{
				var endPosition = new Vector(rand.Next(matrix.R), rand.Next(matrix.R), rand.Next(matrix.R));
				while (forbidden.Contains(endPosition) || matrix.IsFull(endPosition))
					endPosition = new Vector(rand.Next(matrix.R), rand.Next(matrix.R), rand.Next(matrix.R));

				mongoOplogWriter.WriteColor(endPosition, "00FF00", 1);
				DateTime d = DateTime.UtcNow;
				var pathBuilder = new BotMoveSearcher(matrix, startPosition, vector => forbidden.Contains(vector), 39, endPosition, groundedChecker);
				pathBuilder.FindPath(out var movePath, out var commands, out var iterations);

				if (movePath == null)
					break;

				Console.WriteLine($"{commands.Count}: {d - DateTime.UtcNow} - {iterations}");

				movePath.ForEach(c => forbidden.Add(c));


				foreach (var vector in movePath)
				{
					mongoOplogWriter.WriteFill(vector);
				}
				mongoOplogWriter.WriteColor(endPosition, "0000FF", 1);

				startPosition = endPosition;
			}


			mongoOplogWriter.Save();
		}
	}
}
