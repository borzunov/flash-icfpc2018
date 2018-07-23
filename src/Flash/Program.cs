using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            var modelFilePath = @"..\..\..\data\models\LA001_tgt.mdl";

            var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));
			//var ai = new GreedyGravityAI(matrix);

			Console.WriteLine("matrix loaded");

			var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("GreedyGravityAI_IsGrounded");
	        var state = State.CreateInitial(matrix.R, mongoOplogWriter);
	        mongoOplogWriter.WriteInitialState(state);

			var groundedChecker = new IsGroundedChecker(state.Matrix);
			
			var figure = new HashSet<Vector>();

	        for (var x = 0; x < matrix.R; x++)
	        for (var y = 0; y < matrix.R; y++)
	        for (var z = 0; z < matrix.R; z++)
	        {
		        var point = new Vector(x, y, z);
		        if (matrix.IsFull(point))
		        {
			        figure.Add(point);
		        }
	        }

	        var r = new Random(0);

			var path = new PathWork(new Vector(0, 0, 0), figure.ToList()[r.Next(figure.Count)], matrix, groundedChecker, 29, 0);

	        var works = new[] {(IWork)path};

	        var simulator = new Simulator();

	        int i = 0;
	        List<ICommand> commands = null;
	        int commandIdx = 0;

			while (true)
	        {
		        if ((commands == null || commandIdx >= commands.Count) && i < works.Length)
		        {
					works[i].DoWork(vector => false, out commands, out var p);
			        i++;
			        commandIdx = 0;
				}

		        if (commands == null || commandIdx >= commands.Count)
				{
					if (state.Bots[0].Pos == new Vector(0, 0, 0))
					{
						commands = new List<ICommand>{new HaltCommand()};
						commandIdx = 0;
					}
					else
					{
						var path1 = new PathWork(state.Bots[0].Pos, new Vector(0, 0, 0), state.Matrix, groundedChecker, 29, 0);
						path1.DoWork(vector => false, out commands, out _);
						commandIdx = 0;
					}
				}

		        mongoOplogWriter.WriteColor(state.Bots[0].Pos, "FF00FF", 0.2);
				if(state.Matrix.IsFull(state.Bots[0].Pos))
					Console.WriteLine();

				simulator.NextStep(state, new Trace(new []{ commands[commandIdx ++]}));

		        if (commands.Count == 1 && commands[0] is HaltCommand)
		        {
			        break;
		        }
	        }
			
			mongoOplogWriter.Save();
			
        }
    }
}
