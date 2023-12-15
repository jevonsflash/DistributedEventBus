using Abp.Events.Bus;

namespace Abp.DistributedEventBus.Events
{
    public abstract class DistributedEventBusHandleEvent : DistributedEventBusEvent
    {
        public DistributedEventArgs DistributedEventArgs { get; private set; }

        public DistributedEventBusHandleEvent(DistributedEventArgs eventArgs)
        {
            DistributedEventArgs = eventArgs;
        }
    }
}
