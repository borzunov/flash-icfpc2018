using System.Collections.Generic;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure
{
    public class Trace : Queue<ICommand>
    {
        public Trace(IEnumerable<ICommand> commands): base(commands)
        {
            //nothing
        }
    }
}