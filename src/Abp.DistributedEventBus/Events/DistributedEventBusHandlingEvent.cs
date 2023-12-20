namespace Abp.DistributedEventBus.Events
{
    public class DistributedEventBusHandlingEvent : DistributedEventBusHandleEvent
    {
        public DistributedEventBusHandlingEvent(DistributedEventArgs eventArgs)
            : base(eventArgs)
        {

        }
    }
}
