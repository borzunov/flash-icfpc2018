using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Infrastructure
{
    public class State
    {
        //TODO: add other fields
        public Trace Trace { get; }
    }

    public class Trace : List<ICommand>
    {
    }

    public interface ICommand
    {
    }

    public class HaltCommand : ICommand
    {
    }

    public interface ICommandSerializer
    {
        void Serialize(ICommand command, Stream streamToWrite);
    }

    public class HaltCommandSerializer : ICommandSerializer
    {
        public void Serialize(ICommand command, Stream streamToWrite)
        {
            streamToWrite.WriteByte(0b1111_1111);
        }
    }

    public class TraceBinarySerializer
    {
        private static Dictionary<Type, ICommandSerializer> commandTypeToSerializer = new Dictionary<Type, ICommandSerializer>
        {
            { typeof(HaltCommand), new HaltCommandSerializer()}
            //TODO: add other
        };

        public static byte[] Serialize(Trace trace)
        {
            var ms = new MemoryStream();
            foreach (var command in trace)
            {

            }

            return ms.ToArray();
        }
    }
}
