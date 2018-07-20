using System;
using System.IO;
using Flash.Infrastructure.Commands;

namespace Flash.Infrastructure.Serializers
{
    public interface ICommandSerializer
    {
        byte[] Serialize(ICommand command);
    }
}