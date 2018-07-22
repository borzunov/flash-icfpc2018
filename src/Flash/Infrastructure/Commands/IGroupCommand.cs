using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Commands
{
    public interface IGroupCommand : ICommand
    {
        Vector NearDistance { get; }
        Vector FarDistance { get; }
    }
}