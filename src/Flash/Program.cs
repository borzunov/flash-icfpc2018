using System;
using System.IO;
using System.Linq;
using Flash.Infrastructure;
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
            var trackFilePath = @"..\..\..\data\track\LA001.nbt";
            var modelFilePath = @"..\..\..\data\models\LA100_tgt.mdl";

            var ai = new FileAI(trackFilePath);

            var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("MixtureTest_1");

			var simulator = new Simulator();
            var size = 30;
            var state = State.CreateInitial(size, mongoOplogWriter);

	        var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));
	        var mixtureBuilder = new ClusterMixtureBuilder(matrix, 10);
	        var mix = mixtureBuilder.BuildClusterMixture();

	        var colors = new[]
	        {
		        "5c4d63",
		        "004000",
		        "e55a76",
		        "c3c7ff",
		        "b2af88",
		        "c39797",
		        "ffc3a0",
		        "468499",
		        "f08080",
		        "e6e6fa",
		        "ff7373",
		        "008080",
		        "4d4d80",
		        "80804d",
		        "d4acb2",
		        "c6b8ce",
		        "382434",
		        "382434",
		        "c6b8ce",
		        "d4acb2",
		        "80804d",
		        "ddd7a5",
		        "ff65a6",
		        "e1cd66",
		        "000080",
		        "cc0000",
		        "485d72",
		        "2f2648",
		        "9d7ff2",
		        "a876e3",
		        "c71585",
		        "ff7d58",
		        "ffc3a0",
		        "fa8072",
		        "cccccc",
		        "eeeeee",
		        "c6e2ff",
		        "b0e0e6",
		        "d3ffce",
		        "e6e6fa",
		        "ff7373",
		        "ffe4e1"
	        };

	        var rand = new Random();

	        foreach (var group in mix)
	        {
		        var color = colors[rand.Next(colors.Length)];

		        foreach (var vertex in group)
		        {
					mongoOplogWriter.WriteColor(vertex, color);
				}
			}
	        mongoOplogWriter.WriteInitialState(state);

	        mongoOplogWriter.Save();
			return;

			mongoOplogWriter.WriteInitialState(state);

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
