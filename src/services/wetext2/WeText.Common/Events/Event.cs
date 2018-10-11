using System;
using System.Collections.Generic;
using System.Text;

namespace WeText.Common.Events
{
    public abstract class Event : IEvent
    {
        public Guid Id { get; } = Guid.NewGuid();

        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
