using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.CommandsSerializers
{
    public class HaltCommandSerializer : ICommandSerializer
    {
        public void Serialize(ICommand command, Stream streamToWrite)
        {
            streamToWrite.WriteByte(0b1111_1111);
        }
    }
}