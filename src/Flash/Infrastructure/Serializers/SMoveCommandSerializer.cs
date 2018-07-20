using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class SMoveCommandSerializer: BaseCommandSerializer<SMoveCommand>
    {
        protected override byte[] Serialize(SMoveCommand command)
        {
            var bytes = BitWriter.Start()
                .WriteZero(2)
                .WriteLcdAxis(command.Direction)
                .WriteZero()
                .WriteOne()
                .WriteZero(2)
                .EndOfFirstByte()
                .WriteZero(3)
                .WriteLinearLongLength(command.Direction)
                .EndOfSecondByte()
                .ToBytes();

            return bytes;
        }
    }
}