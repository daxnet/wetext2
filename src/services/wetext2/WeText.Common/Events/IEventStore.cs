using System;
using System.Threading.Tasks;

namespace WeText.Common.Events
{
    public interface IEventStore : IDisposable
    {
        Task SaveEventAsync<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}