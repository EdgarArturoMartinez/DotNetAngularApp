using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Application.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Entities.VegProducts> VegProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.VegProducts>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
