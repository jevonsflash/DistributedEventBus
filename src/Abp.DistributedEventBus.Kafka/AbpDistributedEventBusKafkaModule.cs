using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.DistributedEventBus.Kafka
{
    [DependsOn(typeof(AbpDistributedEventBusModule))]
    public class AbpDistributedEventBusKafkaModule: AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpDistributedEventBusKafkaModule).GetAssembly());
        }
    }
}