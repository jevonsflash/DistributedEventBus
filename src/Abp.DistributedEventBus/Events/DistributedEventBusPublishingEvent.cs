namespace Abp.DistributedEventBus.Events
{
    public class DistributedEventBusPublishingEvent: DistributedEventBusPublishEvent
    {
        public DistributedEventBusPublishingEvent(IDistributedEventData eventData)
            : base(eventData)
        {

        }
    }
}
