using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.DistributedEventBus.Redis
{
    [DependsOn(typeof(AbpDistributedEventBusModule))]
    public class AbpDistributedEventBusRedisModule: AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpDistributedEventBusRedisModule).GetAssembly());
        }
    }
}