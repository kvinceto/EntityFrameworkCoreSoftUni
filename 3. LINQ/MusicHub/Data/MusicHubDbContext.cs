namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Producer> Producers { get; set; } = null!;

        public DbSet<Album> Albums { get; set; } = null!;

        public DbSet<Writer> Writers { get; set; } = null!;

        public DbSet<Performer> Performers { get; set; } = null!;

        public DbSet<Song> Songs { get; set; } = null!;

        public DbSet<SongPerformer> SongsPerformers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Producer>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasColumnType("varchar(30)");
            });

            builder.Entity<Album>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasColumnType("varchar(40)");
                entity.Property(a => a.ReleaseDate).IsRequired().HasColumnType("datetime");
                entity.Property(a => a.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity
                .HasOne(a => a.Producer)
                .WithMany(p => p.Albums)
                .HasForeignKey(a => a.ProducerId)
                .IsRequired(false);
            });

            builder.Entity<Performer>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FirstName).IsRequired().HasColumnType("varchar(20)");
                entity.Property(p => p.LastName).IsRequired().HasColumnType("varchar(20)");
                entity.Property(p => p.Age).IsRequired();
                entity.Property(p => p.NetWorth).IsRequired().HasColumnType("decimal(18,2)");
            });

            builder.Entity<Writer>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.Name).IsRequired().HasColumnType("varchar(20)");
            });

            builder.Entity<Song>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasColumnType("varchar(20)");
                entity.Property(s => s.Duration).IsRequired();
                entity.Property(s => s.CreatedOn).IsRequired();
                entity.Property(s => s.Genre).IsRequired();
                entity
                .HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId);
                entity
                .HasOne(s => s.Writer)
                .WithMany(w => w.Songs)
                .HasForeignKey(s => s.WriterId)
                .IsRequired();
                entity.Property(s => s.Price).IsRequired().HasColumnType("decimal(18,2)");
            });

            builder.Entity<SongPerformer>(entity =>
            {
                entity
                .HasOne(sp => sp.Song)
                .WithMany(s => s.SongPerformers)
                .HasForeignKey(sp => sp.SongId)
                .IsRequired();
                entity
                .HasOne(sp => sp.Performer)
                .WithMany(p => p.PerformerSongs)
                .HasForeignKey(sp => sp.PerformerId)
                .IsRequired();
                entity.HasKey(sp => new { sp.SongId, sp.PerformerId });
            });
        }
    }
}
