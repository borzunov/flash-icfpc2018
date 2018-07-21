using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class FissionCommandSerializer : BaseCommandSerializer<FissionCommand>
    {
        protected override byte[] Serialize(FissionCommand command)
        {
            return BitWriter.Start()
                .WriteNearDifference(command.NearDistance)
                .WriteOne()
                .WriteZero()
                .WriteOne()
                .EndOfFirstByte()
                .WriteByte((byte)command.M, 7, 8)
                .EndOfSecondByte()
                .ToBytes();
        }
    }
}