using System.Collections.Generic;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.AI
{
    public interface IAI
    {
        IEnumerable<ICommand> NextStep(State state);
    }
}