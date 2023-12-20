using Abp.Events.Bus;

namespace Abp.DistributedEventBus.Events
{
    public class DistributedEventBusPublishedEvent : DistributedEventBusPublishEvent
    {
        public DistributedEventBusPublishedEvent(IDistributedEventData eventData)
            : base(eventData)
        {

        }
    }
}
