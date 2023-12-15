using Abp.Configuration.Startup;

namespace Abp.DistributedEventBus
{
    public interface IDistributedEventBusConfiguration
    {
        IAbpStartupConfiguration AbpStartupConfiguration { get; }

        IDistributedEventBusConfiguration AutoSubscribe();
    }
}