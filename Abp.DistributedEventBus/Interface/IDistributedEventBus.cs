﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DistributedEventBus
{
    public interface IDistributedEventBus : IDisposable
    {
        void MessageHandle(string topic, string message);

        void Publish(IDistributedEventData eventData);

        void Publish(string topic, IDistributedEventData eventData);

        Task PublishAsync(IDistributedEventData eventData);

        Task PublishAsync(string topic, IDistributedEventData eventData);

        void Subscribe(string topic);

        void Subscribe(IEnumerable<string> topics);

        Task SubscribeAsync(string topic);

        Task SubscribeAsync(IEnumerable<string> topics);

        void Unsubscribe(string topic);

        void Unsubscribe(IEnumerable<string> topics);

        Task UnsubscribeAsync(string topic);

        Task UnsubscribeAsync(IEnumerable<string> topics);

        void UnsubscribeAll();

        Task UnsubscribeAllAsync();
    }
}