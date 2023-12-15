using System;

namespace Abp.DistributedEventBus
{
    public class NullDistributedEventHandler : IDistributedEventHandler
    {
        public static NullDistributedEventHandler Instance { get { return SingletonInstance; } }
        private static readonly NullDistributedEventHandler SingletonInstance = new NullDistributedEventHandler();

        public void HandleEvent(DistributedEventArgs eventData)
        {
            
        }
    }
}
