using System.Collections.Generic;
using System.Linq;
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

	public class Trace1
	{
		public List<ICommand> commands;
		public Trace1(IEnumerable<ICommand> commands)
		{
			this.commands = commands.ToList();
		}

		public Trace Revert()
		{
			var newl = new List<ICommand>();
			foreach (var command in commands)
			{
				newl.Add(command.Revert());
			}

			return new Trace(newl);
		}
	}
}