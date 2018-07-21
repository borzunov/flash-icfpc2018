using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class GVoidCommandSerializer : BaseCommandSerializer<GVoidCommand>
    {
        protected override byte[] Serialize(GVoidCommand command)
        {
            var bytes = BitWriter.Start()
                .WriteNearDifference(command.NearDistance)
                .WriteZero(3)
                .EndOfFirstByte()
                .WriteByte((byte)(command.FarDistance.X + 30), 7, 8)
                .EndOfSecondByte()
                .WriteByte((byte)(command.FarDistance.Y + 30), 7, 8)
                .EndOfThridByte()
                .WriteByte((byte)(command.FarDistance.Z + 30), 7, 8)
                .EndOfFourthByte()
                .ToBytes();

            return bytes;
        }
    }
}