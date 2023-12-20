using System;
using Abp.Dependency;
using Abp.DistributedEventBus.Kafka;
using Castle.MicroKernel.Registration;

namespace Abp.DistributedEventBus.Kafka
{
    public static class ConfigurationExtensions
    {
        public static IKafkaConfiguration UseKafka(this IDistributedEventBusConfiguration configuration)
        {
            var iocManager = configuration.AbpStartupConfiguration.IocManager;

            iocManager.IocContainer.Register(
                Component.For<IDistributedEventProducer>().ImplementedBy<KafkaDistributedEventProducer>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventSubscriber>().ImplementedBy<KafkaDistributedEventSubscriber>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventBus>()
                    .ImplementedBy<DistributedEventBus>()
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            iocManager.RegisterIfNot<IKafkaConfiguration, KafkaConfiguration>();

            return iocManager.Resolve<IKafkaConfiguration>();
        }
    }
}