using Abp.Modules;
using Abp.Reflection.Extensions;
using Commons.Pool;

namespace Abp.DistributedEventBus.RabbitMQ
{
    [DependsOn(typeof(AbpDistributedEventBusModule))]
    public class AbpDistributedEventBusRabbitMQModule: AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IPoolManager, PoolManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpDistributedEventBusRabbitMQModule).GetAssembly());
        }
    }
}