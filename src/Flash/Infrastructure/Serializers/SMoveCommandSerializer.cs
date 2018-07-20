using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class SMoveCommandSerializer : ICommandSerializer
    {
        public Type CommandType => typeof(SMoveCommand);

        public void Serialize(ICommand command, Stream streamToWrite)
        {
            var moveCmd = command as SMoveCommand;
            var bitSet = new BitSet();
        }
    }
}