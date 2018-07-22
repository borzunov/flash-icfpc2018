using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class VoidCommandSerializer : BaseCommandSerializer<VoidCommand>
    {
        protected override byte[] Serialize(VoidCommand command)
        {
            return BitWriter.Start()
                .WriteNearDifference(command.NearDistance)
                .WriteZero()
                .WriteOne()
                .WriteZero()
                .EndOfFirstByte()
                .ToBytes();
        }
    }
}