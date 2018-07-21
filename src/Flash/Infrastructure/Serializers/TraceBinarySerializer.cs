using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Serializers
{
    public class TraceBinarySerializer
    {
        private readonly Dictionary<Type, ICommandSerializer> commandTypeToSerializer;

        public TraceBinarySerializer(IEnumerable<ICommandSerializer> serializers)
        {
            commandTypeToSerializer = serializers.ToDictionary(x => GetCommandType(x.GetType()), x => x);
        }

        public static TraceBinarySerializer Create()
        {
            // ReSharper disable once PossibleNullReferenceException
            var commandTypeToSerializer = Assembly
                .GetAssembly(typeof(TraceBinarySerializer))
                .GetTypes()
                .Where(type => !type.IsAbstract && typeof(ICommandSerializer).IsAssignableFrom(type))
                .Select(x => x.GetConstructor(new Type[0]).Invoke(new object[0]))
                .Cast<ICommandSerializer>();

            return new TraceBinarySerializer(commandTypeToSerializer);
        }

        private Type GetCommandType(Type serializerType)
        {
            // ReSharper disable once PossibleNullReferenceException
            return serializerType.BaseType.GetGenericArguments().First();
        }

        public byte[] Serialize(Trace trace)
        {
            var ms = new MemoryStream();
            foreach (var command in trace)
            {
                var bytes = commandTypeToSerializer[command.GetType()].Serialize(command);
                ms.Write(bytes, 0, bytes.Length);
            }

            return ms.ToArray();
        }
    }
}