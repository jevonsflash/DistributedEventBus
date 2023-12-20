using System;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Abp.DistributedEventBus.RabbitMQ
{
    public static class ConfigurationExtensions
    {
        public static IRabbitMQConfiguration UseRabbitMQ(this IDistributedEventBusConfiguration configuration)
        {
            var iocManager = configuration.AbpStartupConfiguration.IocManager;
            
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventProducer>().ImplementedBy<RabbitMQDistributedEventProducer>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventSubscriber>().ImplementedBy<RabbitMQDistributedEventSubscriber>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventBus>()
                    .ImplementedBy<DistributedEventBus>()
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            iocManager.RegisterIfNot<IRabbitMQConfiguration, RabbitMQConfiguration>();

            return iocManager.Resolve<IRabbitMQConfiguration>();
        }
    }
}