﻿using System;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Abp.DistributedEventBus.Kafka
{
    public class KafkaConfiguration:IKafkaConfiguration, ISingletonDependency
    {
        private readonly IAbpStartupConfiguration _configuration;

        public KafkaConfiguration(IAbpStartupConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IKafkaConfiguration Configure(Action<IKafkaSetting> configureAction)
        {
            _configuration.IocManager.RegisterIfNot<IKafkaSetting, KafkaSetting>();

            var setting = _configuration.IocManager.Resolve<IKafkaSetting>();
            configureAction(setting);

            Configure(setting);

            return this;
        }

        public IKafkaConfiguration Configure(IKafkaSetting setting)
        {
            _configuration.IocManager.IocContainer.Register(
                  Component.For<IDistributedEventProducer>()
                     .ImplementedBy<KafkaDistributedEventProducer>()
                     .DependsOn(Castle.MicroKernel.Registration.Dependency.OnValue<IKafkaSetting>(setting))
                     .Named(Guid.NewGuid().ToString())
                     .LifestyleSingleton()
                     .IsDefault()
             );

            _configuration.IocManager.IocContainer.Register(
                 Component.For<IDistributedEventSubscriber>()
                    .ImplementedBy<KafkaDistributedEventSubscriber>()
                    .DependsOn(Castle.MicroKernel.Registration.Dependency.OnValue<IKafkaSetting>(setting))
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            return this;
        }
    }
}
