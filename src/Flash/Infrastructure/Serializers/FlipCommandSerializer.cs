using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class FlipCommandSerializer : ICommandSerializer
    {
        public Type CommandType => typeof(FlipCommand);

        public void Serialize(ICommand command, Stream streamToWrite)
        {
            streamToWrite.WriteByte(0b1111_1101);
        }
    }
}