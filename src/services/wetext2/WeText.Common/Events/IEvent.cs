using System;

namespace WeText.Common.Events
{
    public interface IEvent
    {
        Guid Id { get; }

        DateTime Timestamp { get; }
    }
}