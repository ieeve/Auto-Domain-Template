using Infrastructure.MemoryBus.Base;
using MediatR;
using Modules.Core.Shared;

namespace Infrastructure.MemoryBus
{
    public sealed class InMemoryBus : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public InMemoryBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task PublishEvent<T>(T @event) where T : Event
        {
            await _mediator.Publish(@event);
        }

        /// <summary>
        /// 发布命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<Result> SendCommand<T>(T command) where T : Command
        {
            return await _mediator.Send(command);
        }
    }
}
