using WeText.Common.Events;
using WeText.Common.Events.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeText.Common
{
    public interface IAggregateRootWithEventSourcing : IAggregateRoot, IPurgable, IPersistedVersionSetter
    {
        IEnumerable<IDomainEvent> UncommittedEvents { get; }

        void Replay(IEnumerable<IDomainEvent> events);

        long Version { get; }
    }
}
