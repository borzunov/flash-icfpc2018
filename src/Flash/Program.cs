using System.Linq;
using Flash.Infrastructure;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Simulation;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var trackFilePath = @"..\..\..\data\track\LA001.nbt";
            var modelFilePath = @"..\..\..\data\track\LA001_tgt.mdl";

            var ai = new FileAI(trackFilePath);

            var mongoOplogWriter = new JsonOpLogWriter(new ConsoleJsonWriter());
            mongoOplogWriter.WriteLogName("MY_BEST_ALGO");

            var simulator = new Simulator();
            var size = 30;
            var state = State.CreateInitial(size, mongoOplogWriter);
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
