using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public interface ICommandSerializer
    {
        Type CommandType { get; }
        void Serialize(ICommand command, Stream streamToWrite);
    }
}