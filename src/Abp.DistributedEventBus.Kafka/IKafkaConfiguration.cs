using System;

namespace Abp.DistributedEventBus.Kafka
{
    public interface IKafkaConfiguration
    {
        IKafkaConfiguration Configure(Action<IKafkaSetting> configureAction);

        IKafkaConfiguration Configure(IKafkaSetting setting);
    }
}
