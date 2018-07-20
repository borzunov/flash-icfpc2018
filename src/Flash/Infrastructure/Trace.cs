using System.Collections.Generic;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure
{
    public class Trace : List<ICommand>
    {
        public Trace(IEnumerable<ICommand> commands): base(commands)
        {
            //nothing
        }
    }
}