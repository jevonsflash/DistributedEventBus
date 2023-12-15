using System;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Abp.DistributedEventBus.Redis
{
    public  static class ConfigurationExtensions
    {
        public static IRedisConfiguration UseRedis(this IDistributedEventBusConfiguration configuration)
        {
            var iocManager = configuration.AbpStartupConfiguration.IocManager;
            
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventPublisher>()
                    .ImplementedBy<RedisDistributedEventPublisher>()
                    .LifestyleSingleton()
                    .IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventSubscriber>()
                    .ImplementedBy<RedisDistributedEventSubscriber>()
                    .LifestyleSingleton()
                    .IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IDistributedEventBus>()
                    .ImplementedBy<DistributedEventBus>()
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            iocManager.RegisterIfNot<IRedisConfiguration, RedisConfiguration>();

            return iocManager.Resolve<IRedisConfiguration>();
        }
    }
}