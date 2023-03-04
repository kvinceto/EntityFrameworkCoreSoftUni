namespace P02_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    using P02_FootballBetting.Data.Models;
    using Config;

    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        { }

        public FootballBettingContext(DbContextOptions options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConectionString);
            }
        }

        public DbSet<Country> Countries { get; set; } = null!;

        public DbSet<Town> Towns { get; set; } = null!;

        public DbSet<Color> Colors { get; set; } = null!;

        public DbSet<Team> Teams { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        public DbSet<Position> Positions { get; set; } = null!;

        public DbSet<PlayerStatistic> PlayersStatistics { get; set; } = null!;

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Bet> Bets { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(c => c.CountryId);
                entity.Property(c => c.Name).IsRequired().HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);
                entity.Property(t => t.Name).IsRequired().HasColumnType("varchar(50)");
                entity
                .HasOne(t => t.Country)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.TownId);
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(c => c.ColorId);
                entity.Property(c => c.Name).IsRequired().HasColumnType("varchar(20)");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);
                entity.Property(t => t.Name).IsRequired().HasColumnType("varchar(100)");
                entity.Property(t => t.LogoUrl).IsRequired().HasColumnType("varchar(250)");
                entity.Property(t => t.Initials).IsRequired().HasColumnType("varchar(10)");
                entity.Property(t => t.Budget).IsRequired().HasColumnType("decimal(18,2)");
                entity
                .HasOne(t => t.PrimaryKitColor)
                .WithMany(c => c.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasOne(t => t.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasOne(t => t.Town)
                .WithMany(to => to.Teams)
                .HasForeignKey(t => t.TownId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);
                entity.Property(p => p.Name).IsRequired().HasColumnType("varchar(50)");
                entity.Property(p => p.SquadNumber).IsRequired();
                entity
                .HasOne(p => p.Town)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TownId);
                entity
                .HasOne(p => p.Position)
                .WithMany(po => po.Players)
                .HasForeignKey(p => p.PositionId);
                entity
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);
                entity.Property(p => p.Name).IsRequired().HasColumnType("varchar(20)");
            });

            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity
                .HasOne(ps => ps.Player)
                .WithMany(p => p.PlayersStatistics)
                .HasForeignKey(ps => ps.PlayerId);
                entity
                .HasOne(ps => ps.Game)
                .WithMany(g => g.PlayersStatistics)
                .HasForeignKey(ps => ps.GameId);
                entity.HasKey(ps => new { ps.PlayerId, ps.GameId });
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.GameId);
                entity.Property(g => g.Result).IsRequired();
                entity
                .HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(b => b.BetId);
                entity.Property(b => b.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(b => b.Prediction).IsRequired();
                entity
                .HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId);
                entity
                .HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Username).IsRequired().HasColumnType("varchar(20)");
                entity.Property(u => u.Password).IsRequired().HasColumnType("varchar(20)");
                entity.Property(u => u.Email).IsRequired().HasColumnType("varchar(50)");
                entity.Property(u => u.Balance).IsRequired().HasColumnType("decimal(18,2)");
            });

        }
    }
}