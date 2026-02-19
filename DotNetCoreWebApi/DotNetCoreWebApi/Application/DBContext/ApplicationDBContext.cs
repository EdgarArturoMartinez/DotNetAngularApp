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
        public DbSet<Entities.VegTypeWeight> VegTypeWeights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure VegCategory primary key
            modelBuilder.Entity<Entities.VegCategory>()
                .HasKey(c => c.IdCategory);

            // Configure VegTypeWeight primary key
            modelBuilder.Entity<Entities.VegTypeWeight>()
                .HasKey(tw => tw.Id);

            // Configure decimal precision for Price
            modelBuilder.Entity<Entities.VegProducts>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Configure decimal precision for NetWeight
            modelBuilder.Entity<Entities.VegProducts>()
                .Property(p => p.NetWeight)
                .HasColumnType("decimal(18,2)");

            // Configure relationship between VegProducts and VegCategory
            modelBuilder.Entity<Entities.VegProducts>()
                .HasOne(p => p.VegCategory)
                .WithMany(c => c.VegProducts)
                .HasForeignKey(p => p.IdCategory)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationship between VegProducts and VegTypeWeight
            modelBuilder.Entity<Entities.VegProducts>()
                .HasOne(p => p.VegTypeWeight)
                .WithMany(tw => tw.VegProducts)
                .HasForeignKey(p => p.IdTypeWeight)
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

            // Seed initial VegTypeWeight data
            modelBuilder.Entity<Entities.VegTypeWeight>().HasData(
                new Entities.VegTypeWeight
                {
                    Id = 1,
                    Name = "Grames",
                    AbbreviationWeight = "Gms",
                    Description = "Grams weight measurement",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Entities.VegTypeWeight
                {
                    Id = 2,
                    Name = "Onzas",
                    AbbreviationWeight = "Oz",
                    Description = "Ounces weight measurement",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Entities.VegTypeWeight
                {
                    Id = 3,
                    Name = "Liters",
                    AbbreviationWeight = "Lts",
                    Description = "Liters volume measurement",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Entities.VegTypeWeight
                {
                    Id = 4,
                    Name = "Kilograms",
                    AbbreviationWeight = "Kg",
                    Description = "Kilograms weight measurement",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Entities.VegTypeWeight
                {
                    Id = 5,
                    Name = "Pounds",
                    AbbreviationWeight = "Lb",
                    Description = "Pounds weight measurement",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Entities.VegTypeWeight
                {
                    Id = 6,
                    Name = "Milliliters",
                    AbbreviationWeight = "ml",
                    Description = "Milliliters volume measurement",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
