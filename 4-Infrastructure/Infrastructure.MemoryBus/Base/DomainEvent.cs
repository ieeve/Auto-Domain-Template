namespace Infrastructure.MemoryBus.Base
{
    public abstract class DomainEvent : Event
    {
        protected DomainEvent(long aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}