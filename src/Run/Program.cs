using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flash.Infrastructure.AI;
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
            Thread.Sleep(1000*10);
            return;

            var tgt = args.Single(a => a.StartsWith("--tgt=")).Substring(6);
            var trace = args.Single(a => a.StartsWith("--trace=")).Substring(8);

            var modelToDraw = MatrixDeserializer.Deserialize(File.ReadAllBytes(tgt));
            var ai = new EasyAI(modelToDraw);
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
