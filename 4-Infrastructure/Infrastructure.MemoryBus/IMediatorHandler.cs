using Infrastructure.MemoryBus.Base;
using Modules.Core.Shared;

namespace Infrastructure.MemoryBus
{
    public interface IMediatorHandler
    {
        Task PublishEvent<T>(T @event) where T : Event;
        Task<Result> SendCommand<T>(T command) where T : Command;
    }
}