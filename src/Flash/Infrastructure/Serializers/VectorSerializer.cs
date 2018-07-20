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
            writerToWrite.WriteByte((byte)(vector.GetFirstNonZeroComponent() + 5));
        }

        public static void SerializeLinearLongLength(Vector vector, BitWriter writerToWrite)
        {
            writerToWrite.WriteByte((byte)(vector.GetFirstNonZeroComponent() + 15));
        }
    }
}
