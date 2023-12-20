using System;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Abp.DistributedEventBus.RabbitMQ
{
    public class RabbitMQConfiguration : IRabbitMQConfiguration, ISingletonDependency
    {
        private readonly IAbpStartupConfiguration _configuration;

        public RabbitMQConfiguration(IAbpStartupConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IRabbitMQConfiguration Configure(Action<IRabbitMQSetting> configureAction)
        {
            _configuration.IocManager.RegisterIfNot<IRabbitMQSetting, RabbitMQSetting>();

            var setting = _configuration.IocManager.Resolve<IRabbitMQSetting>();
            configureAction(setting);

            Configure(setting);

            return this;
        }


        public IRabbitMQConfiguration Configure(IRabbitMQSetting setting)
        {
            _configuration.IocManager.IocContainer.Register(
                 Component.For<IDistributedEventProducer>()
                    .ImplementedBy<RabbitMQDistributedEventProducer>()
                    .DependsOn(Castle.MicroKernel.Registration.Dependency.OnValue<IRabbitMQSetting>(setting))
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            _configuration.IocManager.IocContainer.Register(
                 Component.For<IDistributedEventSubscriber>()
                    .ImplementedBy<RabbitMQDistributedEventSubscriber>()
                    .DependsOn(Castle.MicroKernel.Registration.Dependency.OnValue<IRabbitMQSetting>(setting))
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            return this;
        }
    }
}
