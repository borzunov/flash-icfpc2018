using System.Collections.Generic;
using System.Windows.Input;

namespace Flash.Infrastructure
{
    public class FileAI : IAI
    {
        private readonly string filePath;

        public FileAI(string filePath)
        {
            this.filePath = filePath; //TODO Add deserializer
        }

        public IEnumerable<ICommand> NextStep(State state)
        {
            return new List<ICommand>();
        }
    }
}
