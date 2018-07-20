using System;
using System.Collections.Generic;
using System.IO;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.CommandsSerializers;

namespace Flash.Infrastructure.Serializers
{
    public class TraceBinarySerializer
    {
        private static readonly Dictionary<Type, ICommandSerializer> CommandTypeToSerializer =
            new Dictionary<Type, ICommandSerializer>
            {
                {typeof(HaltCommand), new HaltCommandSerializer()}
                //TODO: add other
            };

        public static byte[] Serialize(Trace trace)
        {
            var ms = new MemoryStream();
            foreach (var command in trace)
            {
                CommandTypeToSerializer[command.GetType()].Serialize(command, ms);
            }

            return ms.ToArray();
        }
    }
}