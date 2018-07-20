using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class LMoveCommandSerializer: BaseCommandSerializer<LMoveCommand>
    {
        protected override byte[] Serialize(LMoveCommand command)
        {
            var bytes = BitWriter.Start()
                .WriteLcdAxis(command.SecondDirection)
                .WriteLcdAxis(command.FirstDirection)
                .WriteOne(2)
                .WriteZero(2)
                .EndOfFirstByte()
                .WriteLinearShortLength(command.SecondDirection)
                .WriteLinearShortLength(command.FirstDirection)
                .EndOfSecondByte()
                .ToBytes();

            return bytes;
        }
    }
}