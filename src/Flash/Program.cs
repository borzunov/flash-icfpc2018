using System.IO;
using System.Linq;
using Flash.Infrastructure;
using Flash.Infrastructure.AI;
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
            var modelFilePath = @"..\..\..\data\models\LA006_tgt.mdl";

            var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));
            var ai = new GreedyGravityAI(matrix);

            var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("GreedyGravityAI_3");

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
