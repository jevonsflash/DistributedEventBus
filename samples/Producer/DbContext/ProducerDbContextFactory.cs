using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Producer.Configuration;
using Producer.Web;

namespace Producer.DbContext
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ProducerDbContextFactory : IDesignTimeDbContextFactory<ProducerDbContext>
    {
        public ProducerDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ProducerDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DbContextConfigurer.ConfigureProducer(builder, configuration.GetConnectionString("Default"));

            return new ProducerDbContext(builder.Options);
        }
    }
}
