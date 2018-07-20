using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public abstract class BaseCommandSerializer<TCommand> : ICommandSerializer
        where TCommand : ICommand
    {
        public byte[] Serialize(ICommand command)
        {
            return Serialize((TCommand)command);
        }

        protected abstract byte[] Serialize(TCommand command);
    }
}