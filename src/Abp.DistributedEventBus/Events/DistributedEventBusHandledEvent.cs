using Abp.Events.Bus;

namespace Abp.DistributedEventBus.Events
{
    public class DistributedEventBusHandledEvent : DistributedEventBusHandleEvent
    {
        public DistributedEventBusHandledEvent(DistributedEventArgs eventArgs)
            : base(eventArgs)
        {

        }
    }
}
