using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Serializers;
using Flash.Infrastructure.Simulation;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            var tgt = args.Single(a => a.StartsWith("--tgt=")).Substring(6);
            var trace = args.Single(a => a.StartsWith("--trace=")).Substring(8);
            
            ///////////////////


            /*var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(File.ReadAllBytes(tgt)));

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
            }*/


            /////////////////////////////////




            var modelToDraw = MatrixDeserializer.Deserialize(File.ReadAllBytes(tgt));

            var tasks = new FigureDecomposer(modelToDraw).Decompose();
            var ai = new GreedyWithFigureDecomposeAI(tasks, new IsGroundedChecker(modelToDraw));
            
            var state = State.CreateInitial(modelToDraw.R);
            var simulator = new Simulator();
            var allCommands = new List<ICommand>();
            while (true)
            {
                var commands = ai.NextStep(state).ToList();
                allCommands.AddRange(commands);
                simulator.NextStep(state, new Trace(commands));

                if (commands.Count == 1 && commands[0] is HaltCommand)
                    break;
            }

            var result = new Trace(allCommands);

            var traceBinarySerializer = TraceBinarySerializer.Create();
            var serialize = traceBinarySerializer.Serialize(result);

            File.WriteAllBytes(trace, serialize);
        }
    }
}
