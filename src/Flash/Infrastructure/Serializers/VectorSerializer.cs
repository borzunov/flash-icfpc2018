using System;

namespace Flash.Infrastructure.Serializers
{
    public class VectorSerializer
    {
        public void SerializeLcdAxis(Vector vector, BitSet setToWrite)
        {
            if (vector.X != 0)
                setToWrite.WriteZero().WriteOne();
            else if (vector.Y != 0)
                setToWrite.WriteOne().WriteZero();
            else
                setToWrite.WriteOne(2);
        }

        public void SerializeLinearShortLength(Vector vector, BitSet setToWrite)
        {
            setToWrite.WriteByte((byte)(vector.Mlen + 5));
        }

        public void SerializeLinearLongLength(Vector vector, BitSet setToWrite)
        {
            setToWrite.WriteByte((byte)(vector.Mlen + 15));
        }


    }
}
