using System;

namespace Flash.Infrastructure.Serializers
{
    public static class VectorSerializer
    {
        public static void SerializeLcdAxis(Vector vector, BitWriter writerToWrite)
        {
            if (vector.X != 0)
                writerToWrite.WriteZero().WriteOne();
            else if (vector.Y != 0)
                writerToWrite.WriteOne().WriteZero();
            else
                writerToWrite.WriteOne(2);
        }

        public static void SerializeLinearShortLength(Vector vector, BitWriter writerToWrite)
        {
            writerToWrite.WriteByte((byte) (vector.GetFirstNonZeroComponent() + 5), 3, 4);
        }

        public static void SerializeLinearLongLength(Vector vector, BitWriter writerToWrite)
        {
            writerToWrite.WriteByte((byte)(vector.GetFirstNonZeroComponent() + 15), 4, 5);
        }
        public static void SerializeNearDifference(Vector vector, BitWriter writerToWrite)
        {
            var nd = (byte) ((vector.X + 1) * 9 + (vector.Y + 1) * 3 + (vector.Z + 1) * 1);
            writerToWrite.WriteByte(nd, 4, 5);
        }
    }
}
