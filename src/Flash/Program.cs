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
using Flash.Infrastructure.Serializers;
using Flash.Infrastructure.Simulation;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var trackFilePath = @"..\..\..\data\track\LA001.nbt";
            var modelFilePath = @"..\..\..\data\models\LA030_tgt.mdl";

            var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));

	        var tasks = new FigureDecomposer(matrix).Decompose();
			var ai = new GreedyWithFigureDecomposeAI(tasks, new IsGroundedChecker(matrix));

			Console.WriteLine("test greedy");

			var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("myTest");
	        var state = State.CreateInitial(matrix.R, mongoOplogWriter);
	        mongoOplogWriter.WriteInitialState(state);
	        
            var simulator = new Simulator();

	        var b = new List<byte>();
            while (true)
            {
                var commands = ai.NextStep(state).ToList();
	            var trace = new Trace(commands);

	            b.AddRange(TraceBinarySerializer.Create().Serialize(trace));

	            simulator.NextStep(state, trace);

                if (commands.Count == 1 && commands[0] is HaltCommand)
                {
                    break;
                }
            }


			File.WriteAllBytes(@"C:\Users\s.jane\Desktop\result\LA019.nbt", b.ToArray());
            mongoOplogWriter.Save();
        }
    }
}
