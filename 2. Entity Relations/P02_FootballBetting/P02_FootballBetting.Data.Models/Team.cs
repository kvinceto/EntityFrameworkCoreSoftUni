namespace P02_FootballBetting.Data.Models
{
    public class Team
    {
        public Team()
        {
            this.Players = new HashSet<Player>();
            this.HomeGames = new HashSet<Game>();
            this.AwayGames = new HashSet<Game>();
        }

        public int TeamId { get; set; }

        public string Name { get; set; } = null!;

        public string LogoUrl { get; set; } = null!;

        public string Initials { get; set; } = null!;

        public decimal Budget { get; set; }

        public int PrimaryKitColorId { get; set; }

        public Color PrimaryKitColor { get; set; } = null!;

        public int SecondaryKitColorId { get; set; }

        public Color SecondaryKitColor { get; set; } = null!;

        public int TownId { get; set; }

        public Town Town { get; set; } = null!;

        public ICollection<Player> Players { get; set; }

        public ICollection<Game> HomeGames { get; set; }

        public ICollection<Game> AwayGames { get; set; }
    }
}