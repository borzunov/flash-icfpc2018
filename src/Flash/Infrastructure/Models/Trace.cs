using System.Collections.Generic;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Models
{
    public class Trace : Queue<ICommand>
    {
        public Trace(IEnumerable<ICommand> commands): base(commands)
        {
            //nothing
        }

        public static Trace From(params ICommand[] commands)
        {
            return new Trace(commands);
        }
    }
}