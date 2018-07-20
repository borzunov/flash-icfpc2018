using System;
using System.Collections.Generic;

namespace Flash.Infrastructure.Serializers
{
    public class BitSet
    {
        private readonly List<byte> bytes = new List<byte> {Capacity = 10};
        private int pointer = 0;

        //private int ByteIndex => pointer + 
        private bool NeedNewByte => pointer / 8 >= bytes.Count;

        public void WriteZero(int count = 1)
        {
            for (var i = 0; i < count; i++)
                Write(0);
        }

        public void WriteOne(int count = 1)
        {
            for (var i = 0; i < count; i++)
                Write(1);
        }

        private void Write(int value)
        {
            if (bytes.Count == 0)
                bytes.Add();
        }
    }
}
