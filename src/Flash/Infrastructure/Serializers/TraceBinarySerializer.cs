using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public class TraceBinarySerializer
    {
        private readonly Dictionary<Type, ICommandSerializer> CommandTypeToSerializer;
        public TraceBinarySerializer()
        {
            CommandTypeToSerializer = Assembly
                .GetAssembly(GetType())
                .GetTypes()
                .Where(type => !type.IsAbstract && typeof(ICommandSerializer).IsAssignableFrom(type))
                .Select(x => x.GetConstructor(new Type[] { })
                .Invoke(new object[0]))
                .Cast<ICommandSerializer>()
                .ToDictionary(x => x.CommandType, x => x);
        }

        public TraceBinarySerializer(IEnumerable<ICommandSerializer> serializers)
        {
            CommandTypeToSerializer = serializers.ToDictionary(x => x.CommandType, x => x);
        }

        public byte[] Serialize(Trace trace)
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