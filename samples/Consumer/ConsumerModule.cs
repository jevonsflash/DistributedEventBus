using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp;
using Microsoft.Extensions.Configuration;
using Abp.DistributedEventBus.Redis;
using Abp.DistributedEventBus;
using Abp.EntityFrameworkCore.Configuration;
using System.Reflection;
using Share.Entites;

namespace Consumer
{
    [DependsOn(typeof(AbpKernelModule))]
    [DependsOn(typeof(AbpDistributedEventBusRedisModule))]
    public class ConsumerModule : AbpModule
    {

        public ConsumerModule()
        {
        }

        public override void PreInitialize()
        {

            Configuration.Modules.DistributedEventBus().UseRedis().Configure(setting =>
            {
                setting.Server = "127.0.0.1:6379";
            });

            //Configuration.Modules.DistributedEventBus().UseRabbitMQ().Configure(setting =>
            //{
            //    setting.Url = "amqp://guest:guest@192.168.1.120:5672/";
            //});


            Configuration.ReplaceService(
             typeof(IEventBus),
             () => IocManager.IocContainer.Register(
                 Component.For<IEventBus>().ImplementedBy<MultipleEventBus>()
             ));
        }


        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ConsumerModule).GetAssembly());
            Console.WriteLine("Initialized!");

        }

        public override Assembly[] GetAdditionalAssemblies()
        {
            var clientModuleAssembly = typeof(Person).GetAssembly();
            return [clientModuleAssembly];
        }
    }
}
