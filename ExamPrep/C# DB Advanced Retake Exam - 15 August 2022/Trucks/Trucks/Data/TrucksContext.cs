namespace Trucks.Data
{
    using Microsoft.EntityFrameworkCore;
    using Trucks.Data.Models;

    public class TrucksContext : DbContext
    {
        public TrucksContext()
        {
        }

        public TrucksContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Despatcher> Despatchers { get; set; } = null!;

        public DbSet<Truck> Trucks { get; set; } = null!;

        public DbSet<Client> Clients { get; set; } = null!;

        public DbSet<ClientTruck> ClientsTrucks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Despatcher>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasColumnType("nvarchar(40)");
                entity.Property(d => d.Position).IsRequired(false).IsUnicode();
            });

            modelBuilder.Entity<Truck>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.RegistrationNumber).IsRequired(false).HasColumnType("char(8)");
                entity.Property(t => t.VinNumber).IsRequired().HasColumnType("char(17)");
                entity.Property(t => t.TankCapacity).IsRequired();
                entity.Property(t => t.CargoCapacity).IsRequired();
                entity.Property(t => t.MakeType).IsRequired().HasColumnType("varchar(8)");
                entity.Property(t => t.CategoryType).IsRequired().HasColumnType("varchar(12)");
                entity
                .HasOne(t => t.Despatcher)
                .WithMany(d => d.Trucks)
                .HasForeignKey(t => t.DespatcherId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasColumnType("nvarchar(40)");
                entity.Property(c => c.Nationality).IsRequired().HasColumnType("nvarchar(40)");
                entity.Property(c => c.Type).IsRequired().IsUnicode();
            });

            modelBuilder.Entity<ClientTruck>(entity =>
            {
                entity.HasKey(ct => new { ct.TruckId, ct.ClientId });
                entity
                .HasOne(ct => ct.Client)
                .WithMany(c => c.ClientsTrucks)
                .HasForeignKey(ct => ct.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
                entity
                .HasOne(ct => ct.Truck)
                .WithMany(t => t.ClientsTrucks)
                .HasForeignKey(ct => ct.TruckId)
                .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
