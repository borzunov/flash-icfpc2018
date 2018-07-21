using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class GFillCommandSerializer : BaseCommandSerializer<GFillCommand>
    {
        protected override byte[] Serialize(GFillCommand command)
        {
            var bytes = BitWriter.Start()
                .WriteNearDifference(command.NearDistance)
                .WriteZero(2)
                .WriteOne()
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