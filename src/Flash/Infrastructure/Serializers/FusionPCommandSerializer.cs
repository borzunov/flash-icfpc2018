using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class FusionPCommandSerializer : BaseCommandSerializer<FusionPCommand>
    {
        protected override byte[] Serialize(FusionPCommand command)
        {
            return BitWriter.Start()
                .WriteNearDifference(command.NearDistance)
                .WriteOne(3)
                .EndOfFirstByte()
                .ToBytes();
        }
    }
}