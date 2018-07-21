using System.Collections.Generic;
using System.Windows.Input;

namespace Flash.Infrastructure
{
    public interface IAI
    {
        IEnumerable<ICommand> NextStep(State state);
    }
}