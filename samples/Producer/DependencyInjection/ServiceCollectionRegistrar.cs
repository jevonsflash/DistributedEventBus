using Microsoft.Extensions.DependencyInjection;
using Abp.Dependency;
using Castle.Windsor.MsDependencyInjection;

namespace Producer.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }
    }
}
