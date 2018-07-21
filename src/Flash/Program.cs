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
            var trackFilePath = @"";
            var ai = new FileAI(trackFilePath);

            var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            var simulator = new Simulator();
            var size = 100;
            var initialState = State.CreateInitial(100, mongoOplogWriter);

            while (true)
            {
                var commands = ai.NextStep(initialState).ToList();
                simulator.NextStep(initialState, new Trace(commands));

                if (commands.Count == 1 && commands[0] is HaltCommand)
                {
                    break;
                }
            }

            mongoOplogWriter.Save();
        }
    }
}
