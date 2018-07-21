using System.Collections.Generic;
using System.IO;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Simulation;

namespace Flash.Infrastructure
{
    public class FileAI : IAI
    {
        private readonly Trace commands;

        public FileAI(string filePath)
        {
            var desserializer = new TraceBinaryDeserializer();
            commands = desserializer.Deserialize(File.ReadAllBytes(filePath));
        }

        public IEnumerable<ICommand> NextStep(State state)
        {
            return commands.Dequeue(state.Bots.Length);
        }
    }
}
