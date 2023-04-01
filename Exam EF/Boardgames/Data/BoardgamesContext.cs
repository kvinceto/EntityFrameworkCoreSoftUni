namespace Boardgames.Data
{
    using Boardgames.Data.Models;
    using Microsoft.EntityFrameworkCore;
    
    public class BoardgamesContext : DbContext
    {
        public BoardgamesContext()
        { 
        }

        public BoardgamesContext(DbContextOptions options)
            : base(options) 
        {
        }

        public DbSet<Boardgame> Boardgames { get; set; } = null!;

        public DbSet<Seller> Sellers { get; set; } = null!;

        public DbSet<Creator> Creators { get; set; } = null!;

        public DbSet<BoardgameSeller> BoardgamesSellers { get; set; } = null!;

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
            modelBuilder.Entity<Boardgame>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name).IsRequired().HasColumnType("nvarchar(20)");
                entity.Property(b => b.Rating).IsRequired();
                entity.Property(b => b.YearPublished).IsRequired();
                entity.Property(b => b.CategoryType).IsRequired().HasColumnType("nvarchar(8)");
                entity.Property(b => b.Mechanics).IsRequired().HasColumnType("nvarchar(max)");
                entity
                 .HasOne(b => b.Creator)
                 .WithMany(c => c.Boardgames)
                 .HasForeignKey(b => b.CreatorId);
            });

            modelBuilder.Entity<Creator>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FirstName).IsRequired().HasColumnType("nvarchar(7)");
                entity.Property(c => c.LastName).IsRequired().HasColumnType("nvarchar(7)");
            });

            modelBuilder.Entity<Seller>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasColumnType("nvarchar(20)");
                entity.Property(s => s.Address).IsRequired().HasColumnType("nvarchar(30)");
                entity.Property(s => s.Country).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(s => s.Website).IsRequired().HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<BoardgameSeller>(entity =>
            {
                entity.HasKey(bs => new { bs.BoardgameId, bs.SellerId });
                entity
                .HasOne(bs => bs.Boardgame)
                .WithMany(b => b.BoardgamesSellers)
                .HasForeignKey(bs => bs.BoardgameId);
                entity
                .HasOne(bs => bs.Seller)
                .WithMany(s => s.BoardgamesSellers)
                .HasForeignKey(bs => bs.SellerId);
            });
        }
    }
}
