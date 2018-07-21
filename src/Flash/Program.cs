using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flash.Infrastructure;
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

            var num = 7;
            var list = Directory.EnumerateFiles(modelFilePath).Skip(num-1).Take(1).ToList();
            //list.Shuffle();

            var count = list.Select(file =>
            {
                var resultMatrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(file));
                var ai = new LineAI(resultMatrix);

                var opLogWriter = new JsonOpLogWriter(new MongoJsonWriter());
                opLogWriter.WriteLogName($"TIME TO WINlol! {num}");

                var simulator = new Simulator();
                var size = resultMatrix.R;
                var state = State.CreateInitial(size, opLogWriter);
                opLogWriter.WriteInitialState(state);

                var count1 = 0;
                try
                {
                    while (true)
                    {
                        /*count1++;
                    if(count1 > 100)
                        break;*/
                        var commands = ai.NextStep(state).ToList();

                        var trace = new Trace(commands);

                        simulator.NextStep(state, trace);

                        if (commands.Count == 1 && commands[0] is HaltCommand)
                        {
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                opLogWriter.Save();

                return 1;
            }).Take(1).Count();
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
