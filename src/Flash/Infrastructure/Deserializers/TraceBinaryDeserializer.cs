using System;
using System.Collections.Generic;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Deserializers
{
    public class TraceBinaryDeserializer
    {
        public Trace Deserialize(byte[] bytes)
        {
            var commands = ProcessBytes(bytes);
            var trace = new Trace(commands);

            return trace;
        }

        private IEnumerable<ICommand> ProcessBytes(byte[] bytes)
        {
            var idx = 0;
            while(idx < bytes.Length)
            {
                var current = bytes[idx];
                var deserializer = GetDeserializer(current);
                var command = deserializer.Desrialize(bytes, idx, out var readBytesCount);
                idx += readBytesCount;

                yield return command;
            }
        }

        private ICommandDeserializer GetDeserializer(byte markerByte)
        {
            //TODO: refactor this in case of a lot of free time
            //it's a kind of a bit magic
            //1
            if (markerByte == 0b1111_1111)
                return new HaltCommandDeserializer();
            if (markerByte == 0b1111_1110)
                return new WaitCommandDeserializer();
            if (markerByte == 0b1111_1101)
                return new FlipCommandDeserializer();

            //2
            if((markerByte & 0b0000_0111) == 0b0000_0111)
                return new FusionPCommandDeserializer();
            if ((markerByte & 0b0000_0111) == 0b0000_0110)
                return new FusionSCommandDeserializer();
            if ((markerByte & 0b0000_0111) == 0b0000_0101)
                return new FissionCommandDeserializer();
            if ((markerByte & 0b0000_0111) == 0b0000_0011)
                return new FillCommandDeserializer();

            //3
            if((markerByte & 0b0000_1111) == 0b0000_0100)
                return new SMoveCommandDeserializer();
            if ((markerByte & 0b0000_1111) == 0b0000_1100)
                return new LMoveCommandDeserializer();

            throw new NotSupportedException($"unknown marker byte '{markerByte:X}'");
        }
    }
}
