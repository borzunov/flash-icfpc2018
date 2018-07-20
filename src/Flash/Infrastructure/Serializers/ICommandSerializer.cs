using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.CommandsSerializers
{
    public interface ICommandSerializer
    {
        void Serialize(ICommand command, Stream streamToWrite);
    }
}