using System;
using System.Collections.Generic;
using System.Text;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Abp.DistributedEventBus.Redis
{
    public class RedisConfiguration : IRedisConfiguration, ISingletonDependency
    {
        private readonly IAbpStartupConfiguration _configuration;

        public RedisConfiguration(IAbpStartupConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IRedisConfiguration Configure(Action<IRedisSetting> configureAction)
        {
            _configuration.IocManager.RegisterIfNot<IRedisSetting, RedisSetting>();

            var setting = _configuration.IocManager.Resolve<IRedisSetting>();
            configureAction(setting);

            Configure(setting);

            return this;
        }

        public IRedisConfiguration Configure(IRedisSetting setting)
        {
            _configuration.IocManager.IocContainer.Register(
                  Component.For<IDistributedEventProducer>()
                     .ImplementedBy<RedisDistributedEventProducer>()
                     .DependsOn(Castle.MicroKernel.Registration.Dependency.OnValue<IRedisSetting>(setting))
                     .Named(Guid.NewGuid().ToString())
                     .LifestyleSingleton()
                     .IsDefault()
             );

            _configuration.IocManager.IocContainer.Register(
                 Component.For<IDistributedEventSubscriber>()
                    .ImplementedBy<RedisDistributedEventSubscriber>()
                    .DependsOn(Castle.MicroKernel.Registration.Dependency.OnValue<IRedisSetting>(setting))
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            return this;
        }
    }
}
