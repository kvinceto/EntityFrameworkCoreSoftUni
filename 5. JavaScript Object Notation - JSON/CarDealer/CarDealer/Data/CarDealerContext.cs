using Microsoft.EntityFrameworkCore;
using CarDealer.Models;

namespace CarDealer.Data
{
    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {
        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Part> Parts { get; set; } = null!;
        public DbSet<PartCar> PartsCars { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().IsUnicode(true);
                entity.Property(s => s.IsImporter).HasDefaultValue(false);
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().IsUnicode(true);
                entity.Property(p => p.Quantity).IsRequired();
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Parts)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<PartCar>(e =>
            {
                e.HasKey(k => new { k.CarId, k.PartId });
                e
                .HasOne(pc => pc.Car)
                .WithMany(c => c.PartsCars)
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.NoAction);
                e
                .HasOne(pc => pc.Part)
                .WithMany(p => p.PartsCars)
                .HasForeignKey(pc => pc.PartId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().IsUnicode(true);
                entity.Property(c => c.BirthDate).IsRequired();
                
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.CarId).IsRequired();
                entity.Property(s => s.CustomerId).IsRequired();
                entity.Property(s => s.Discount).IsRequired().HasColumnType("decimal(18,2)");
                entity
                .HasOne(s => s.Car)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CarId)
                .OnDelete(DeleteBehavior.NoAction);
                entity
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Make).IsRequired().IsUnicode(true);
                entity.Property(c => c.Model).IsRequired().IsUnicode(true);
                entity.Property(c => c.TraveledDistance).IsRequired().HasColumnType("bigint");

            });
        }
    }
}
