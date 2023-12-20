using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Producer.DbContext;

namespace Producer
{
    public static class DbContextConfigurer
    {


        public static void ConfigureProducer(DbContextOptionsBuilder<ProducerDbContext> builder, string connectionString)
        {
            builder.UseInMemoryDatabase(connectionString);
        }

        public static void ConfigureProducer(DbContextOptionsBuilder<ProducerDbContext> builder, DbConnection connection)
        {
            builder.UseInMemoryDatabase(connection.ConnectionString);
        }

    }
}
