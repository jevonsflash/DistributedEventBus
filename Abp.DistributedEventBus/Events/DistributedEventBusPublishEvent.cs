using Abp.Events.Bus;

namespace Abp.DistributedEventBus.Events
{
    public abstract class DistributedEventBusPublishEvent : DistributedEventBusEvent
    {
        public IDistributedEventData DistributedEventData { get; private set; }

        public DistributedEventBusPublishEvent(IDistributedEventData eventData)
        {
            DistributedEventData = eventData;
        }
    }
}
