using System;

namespace WeText.Common.Events
{
    public interface IEventSubscriber : IDisposable
    {
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>;
    }
}