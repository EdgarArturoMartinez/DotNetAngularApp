using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Application.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Entities.VegProducts> VegProducts { get; set; }
        public DbSet<Entities.VegCategory> VegCategories { get; set; }
        public DbSet<Entities.ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure VegCategory primary key
            modelBuilder.Entity<Entities.VegCategory>()
                .HasKey(c => c.IdCategory);

            // Configure decimal precision for Price
            modelBuilder.Entity<Entities.VegProducts>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Configure relationship between VegProducts and VegCategory
            modelBuilder.Entity<Entities.VegProducts>()
                .HasOne(p => p.VegCategory)
                .WithMany(c => c.VegProducts)
                .HasForeignKey(p => p.IdCategory)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationship between VegProducts and ProductImage
            modelBuilder.Entity<Entities.ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.IdProduct)
                .OnDelete(DeleteBehavior.Cascade); // Delete images when product is deleted

            // Configure ProductImage constraints
            modelBuilder.Entity<Entities.ProductImage>()
                .HasIndex(pi => pi.IdProduct);

            modelBuilder.Entity<Entities.ProductImage>()
                .Property(pi => pi.UploadedDate)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
