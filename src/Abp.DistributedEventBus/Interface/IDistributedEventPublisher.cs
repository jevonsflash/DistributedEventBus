﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.DistributedEventBus
{
    public interface IDistributedEventProducer: IDisposable
    {
        void Publish(string topic, IDistributedEventData remoteEventData);

        Task PublishAsync(string topic, IDistributedEventData remoteEventData);
    }
}
