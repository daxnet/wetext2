using System;

namespace WeText.Common.Events.Domain
{
    public interface IDomainEvent : IEvent
    {
        Guid AggregateRootId { get; set; }
        string AggregateRootType { get; set; }
        long Sequence { get; set; }
    }
}