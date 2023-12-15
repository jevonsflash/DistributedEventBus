using Abp.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DistributedEventBus
{
    public static class ConfigurationExtensions
    {
        public static IDistributedEventBusConfiguration DistributedEventBus(this IModuleConfigurations configuration)
        {
            return configuration.AbpConfiguration.GetOrCreate("Modules.Abp.DistributedEventBus", () => configuration.AbpConfiguration.IocManager.Resolve<IDistributedEventBusConfiguration>());
        }
    }
}
