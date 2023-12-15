using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.DistributedEventBus
{
    public class NullDistributedEventBus : IDistributedEventBus
    {
        public static NullDistributedEventBus Instance { get { return SingletonInstance; } }
        private static readonly NullDistributedEventBus SingletonInstance = new NullDistributedEventBus();

        public void Dispose()
        {
            
        }

        public void MessageHandle(string topic, string message)
        {
            
        }

        public void Publish(IDistributedEventData eventData)
        {
            
        }

        public void Publish(string topic, IDistributedEventData eventData)
        {
            
        }

        public Task PublishAsync(IDistributedEventData eventData)
        {
            return Task.FromResult(0);
        }

        public Task PublishAsync(string topic, IDistributedEventData eventData)
        {
            return Task.FromResult(0);
        }

        public void Subscribe(string topic)
        {
            
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            
        }

        public Task SubscribeAsync(string topic)
        {
            return Task.FromResult(0);
        }

        public Task SubscribeAsync(IEnumerable<string> topics)
        {
            return Task.FromResult(0);
        }

        public void Unsubscribe(string topic)
        {
            
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            
        }

        public void UnsubscribeAll()
        {
            
        }

        public Task UnsubscribeAllAsync()
        {
            return Task.FromResult(0);
        }

        public Task UnsubscribeAsync(string topic)
        {
            return Task.FromResult(0);
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return Task.FromResult(0);
        }
    }
}
