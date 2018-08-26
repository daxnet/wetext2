using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeText.Common.Events
{
    public interface IEventPublisher : IDisposable
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}