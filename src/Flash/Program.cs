using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flash.Infrastructure;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Simulation;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var trackFilePath = @"..\..\..\data\track\LA001.nbt";
            var modelFilePath = @"..\..\..\data\models\LA020_tgt.mdl";

            var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));
            var ai = new GreedyGravityAI(matrix);

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

			return;

            var simulator = new Simulator();

            while (true)
            {
                var commands = ai.NextStep(state).ToList();
                simulator.NextStep(state, new Trace(commands));

                if (commands.Count == 1 && commands[0] is HaltCommand)
                {
                    break;
                }
            }

            mongoOplogWriter.Save();
        }
    }
}
