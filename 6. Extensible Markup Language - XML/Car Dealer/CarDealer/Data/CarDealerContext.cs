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
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().IsUnicode();
                entity.Property(s => s.IsImporter).IsRequired();
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().IsUnicode();
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(p => p.Quantity).IsRequired();
                entity
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Parts)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().IsUnicode();
                entity.Property(c => c.BirthDate).IsRequired();
                entity.Property(c => c.IsYoungDriver).IsRequired();
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Discount)
                .IsRequired().HasColumnType("decimal(18,2)");
                entity
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
                entity
                .HasOne(s => s.Car)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CarId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Make).IsRequired().IsUnicode();
                entity.Property(c => c.Model).IsRequired().IsUnicode();
                entity.Property(c => c.TravelledDistance).IsRequired();
            });

            modelBuilder.Entity<PartCar>(entity =>
            {
                entity.HasKey(k => new { k.CarId, k.PartId });
                entity
                .HasOne(pc => pc.Part)
                .WithMany(p => p.PartsCars)
                .HasForeignKey(pc => pc.PartId)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasOne(pc => pc.Car)
                .WithMany(c => c.PartsCars)
                .HasForeignKey(pc => pc.CarId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
