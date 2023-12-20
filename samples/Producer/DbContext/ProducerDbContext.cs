using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Share.Entites;


#nullable disable

namespace Producer.DbContext
{
    public sealed partial class ProducerDbContext : AbpDbContext
    {
        private readonly string schema = "us_pos";

        public ProducerDbContext(DbContextOptions<ProducerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> ProducerPeople { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.HasDefaultSchema(schema);
            base.OnModelCreating(builder);
        }



    }
}
