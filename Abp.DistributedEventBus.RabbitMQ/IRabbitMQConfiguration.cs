using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DistributedEventBus.RabbitMQ
{
    public interface IRabbitMQConfiguration
    {
        IRabbitMQConfiguration Configure(Action<IRabbitMQSetting> configureAction);

        IRabbitMQConfiguration Configure(IRabbitMQSetting setting);
    }
}
