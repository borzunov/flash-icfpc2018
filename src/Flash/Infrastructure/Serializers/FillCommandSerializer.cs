using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class FillCommandSerializer : BaseCommandSerializer<FillCommand>
    {
        protected override byte[] Serialize(FillCommand command)
        {
            return BitWriter.Start()
                .WriteNearDifference(command.NearDistance)
                .WriteZero()
                .WriteOne(2)
                .EndOfFirstByte()
                .ToBytes();
        }
    }
}