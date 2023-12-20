using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp;
using Microsoft.Extensions.Configuration;
using Abp.DistributedEventBus.Redis;
using Abp.DistributedEventBus;
using Abp.EntityFrameworkCore.Configuration;
using Producer.Configuration;
using Producer.DependencyInjection;
using Producer.DbContext;
using Abp.EntityFrameworkCore;

namespace Producer
{
    [DependsOn(typeof(AbpKernelModule))]
    [DependsOn(typeof(AbpDistributedEventBusRedisModule))]
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class ProducerModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public ProducerModule()
        {
            _appConfiguration = AppConfigurations.Get(
                typeof(ProducerModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false;
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                "Default");
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            IocManager.IocContainer.Register(
                  Component.For<IDistributedEventTopicSelector>().ImplementedBy<MultipleEventTopicSelector>().IsDefault());
            Configuration.Modules.AbpEfCore().AddDbContext<ProducerDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    DbContextConfigurer.ConfigureProducer(options.DbContextOptions, options.ExistingConnection);
                }
                else
                {
                    DbContextConfigurer.ConfigureProducer(options.DbContextOptions, options.ConnectionString);
                }
            });



            Configuration.Modules.DistributedEventBus().UseRedis().Configure(setting =>
            {
                setting.Server = "127.0.0.1:6379";
            });

            //Configuration.Modules.DistributedEventBus().UseRabbitMQ().Configure(setting =>
            //{
            //    setting.Url = "amqp://guest:guest@192.168.1.120:5672/";
            //});


            //todo: 事件总线
            Configuration.ReplaceService(
             typeof(IEventBus),
             () => IocManager.IocContainer.Register(
                 Component.For<IEventBus>().ImplementedBy<MultipleEventBus>()
             ));
        }


        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ProducerModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);

            Console.WriteLine("Initialized!");

        }
    }
}
