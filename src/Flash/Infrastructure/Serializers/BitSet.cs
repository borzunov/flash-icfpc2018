using System;
using System.Collections.Generic;

namespace Flash.Infrastructure.Serializers
{
    public class BitSet
    {
        private readonly List<byte> bytes = new List<byte> {Capacity = 10};
        private int bytePointer;
        private int bitPointer;

        public BitSet WriteByte(byte value)
        {
            for (var bit = 7; bit >= 0; bit--)
            {
                if ((value & (1 << bit)) != 0)
                    WriteOne();
                else
                    WriteZero();
            }

            return this;
        }

        public BitSet WriteZero(int count = 1)
        {
            for (var i = 0; i < count; i++)
                GoToNextBit();

            return this;
        }

        public BitSet WriteOne(int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                GoToNextBit();
                bytes[bytePointer] |= (byte) (1 << bitPointer);
            }

            return this;
        }

        public byte[] ToBytes()
        {
            return bytes.ToArray();
        }

        private void GoToNextBit()
        {
            if (bitPointer == 0)
                GoToNextByte();
            else
                bitPointer--;
        }

        private void GoToNextByte()
        {
            bytes.Add(0);
            bytePointer++;
            bitPointer = 7;
        }
    }
}
