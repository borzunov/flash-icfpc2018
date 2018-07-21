using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Infrastructure.Deserializers
{
    public static class VectorDeserializer
    {
        public static Vector DeserializeLcdAxis(byte @byte)
        {
            if (@byte == 0b0000_0001)
                return new Vector(1, 0, 0);
            if (@byte == 0b0000_0010)
                return new Vector(0, 1, 0);
            if (@byte == 0b0000_0011)
                return new Vector(0, 0, 1);

            throw new NotSupportedException($"Can't deserialize LcdAxis by '{@byte:X}'");
        }

        public static int DeserializeLinearShortLength(byte @byte)
        {
            return @byte - 5;
        }

        public static int DeserializeLinearLongLength(byte @byte)
        {
            return @byte - 15;
        }

        public static Vector DeserializeNearDifference(byte @byte)
        {
            if (NdByteToVector.TryGetValue(@byte, out var vector))
                return vector;

            throw new NotSupportedException($"Cant deserialize nd from {@byte:X}");
        }

        private static readonly Dictionary<byte, Vector> NdByteToVector =
            new Vector(0, 0, 0).GetNears()
                .ToDictionary(
                    v => (byte) ((v.X + 1) * 9 + (v.Y + 1) * 3 + (v.Z + 1) * 1),
                    v => v);
    }
}
