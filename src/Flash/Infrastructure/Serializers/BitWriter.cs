using System;
using System.Collections.Generic;
using System.IO;

namespace Flash.Infrastructure.Serializers
{
    public class BitWriter
    {
        private readonly List<byte> bytes = new List<byte> {Capacity = 10};
        private int bytePointer = -1;
        private int bitPointer;
        private int finishedBytesCount;

        public static BitWriter Start()
        {
            var bitWriter = new BitWriter();

            return bitWriter;
        }

        public BitWriter WriteByte(byte value, int offset, int length)
        {
            for (var bit = offset; bit >= Math.Max(offset - length + 1, 0); bit--)
            {
                if ((value & (1 << bit)) != 0)
                    WriteOne();
                else
                    WriteZero();
            }

            return this;
        }

        public BitWriter WriteZero(int count = 1)
        {
            for (var i = 0; i < count; i++)
                GoToNextBit();

            return this;
        }

        public BitWriter WriteOne(int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                GoToNextBit();
                bytes[bytePointer] |= (byte) (1 << bitPointer);
            }

            return this;
        }

        public BitWriter EndOfFirstByte()
        {
            return EndByte(1);
        }

        public BitWriter EndOfSecondByte()
        {
            return EndByte(2);
        }

        private BitWriter EndByte(int n)
        {
            if (bytePointer != n-1 || bitPointer != 0)
                throw new InvalidOperationException("Incorrect number of bits");

            finishedBytesCount++;

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
            if(bytePointer + 1 != finishedBytesCount)
                throw new InvalidOperationException("No enought end labels");

            bytes.Add(0);
            bytePointer++;
            bitPointer = 7;
        }
    }
}
