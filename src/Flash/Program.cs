using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Flash.Infrastructure;
using Flash.Infrastructure.AI;
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
            var trackFilePath = @"..\..\..\data\track\LA001.nbt";
            var modelFilePath = @"..\..\..\data\models\";


            var list = Directory.EnumerateFiles(modelFilePath).Skip(30).ToList();
            list.Shuffle();

            var count = list.AsParallel().Select(file =>
            {
                var resultMatrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(file));
                var ai = new GreedyAI();



                var simulator = new Simulator();
                var size = resultMatrix.R;
                var state = State.CreateInitial(size, new FakeOpLog());
                state.Matrix = resultMatrix;

                var ans = new List<Trace1>();
                while (true)
                {
                    var commands = ai.NextStep(state).ToList();

                    var trace = new Trace(commands);
                    var trace1 = new Trace1(commands);

                    simulator.NextStep(state, trace);
                    ans.Add(trace1);

                    if (commands.Count == 1 && commands[0] is HaltCommand)
                    {
                        break;
                    }
                }

                var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
                mongoOplogWriter.WriteLogName($"{Path.GetFileName(file).Substring(0, 5)}");

                var newState = State.CreateInitial(size, mongoOplogWriter);
                mongoOplogWriter.WriteInitialState(newState);

                var traceBinarySerializer = TraceBinarySerializer.Create();
                var bytes = new List<byte>();
                for (var index = ans.Count - 2; index >= 0; index--)
                {
                    var trace1 = ans[index];
                    var newTrace = trace1.Revert();
                    bytes.AddRange(traceBinarySerializer.Serialize(newTrace));
                    simulator.NextStep(newState, newTrace);
                }

                Console.WriteLine($"{Path.GetFileName(file).Substring(0, 5)}  - {newState.Energy}");
                var halt = new Trace(new ICommand[] { new HaltCommand() });
                bytes.AddRange(traceBinarySerializer.Serialize(halt));
                simulator.NextStep(newState, halt);

                File.WriteAllBytes($"result\\{Path.GetFileName(file).Substring(0, 5)}.nbt", bytes.ToArray());
                mongoOplogWriter.Save();

                return 1;
            }).Count();
        }

    }

    public static class ListEx
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
