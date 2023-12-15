using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.DistributedEventBus
{
    public class AbpDistributedEventBusModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpDistributedEventBusModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IDistributedEventBus, NullDistributedEventBus>();
        }
    }
}