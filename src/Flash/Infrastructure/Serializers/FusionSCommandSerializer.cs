using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class FusionSCommandSerializer : BaseCommandSerializer<FusionSCommand>
    {
        protected override byte[] Serialize(FusionSCommand command)
        {
            return BitWriter.Start()
                .WriteNearDifference(command.NearDistance)
                .WriteOne(2)
                .WriteZero()
                .EndOfFirstByte()
                .ToBytes();
        }
    }
}