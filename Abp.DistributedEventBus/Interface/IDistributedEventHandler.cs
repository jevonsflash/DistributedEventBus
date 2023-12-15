namespace Abp.DistributedEventBus
{
    public interface IDistributedEventHandler
    {
        void HandleEvent(DistributedEventArgs eventArgs);
    }
}
