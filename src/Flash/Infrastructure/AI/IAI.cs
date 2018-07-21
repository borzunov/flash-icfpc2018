using System.Collections.Generic;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure
{
    public interface IAI
    {
        IEnumerable<ICommand> NextStep(State state);
    }
}