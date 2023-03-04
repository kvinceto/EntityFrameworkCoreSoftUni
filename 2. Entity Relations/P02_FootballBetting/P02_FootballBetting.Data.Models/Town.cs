namespace P02_FootballBetting.Data.Models
{
    public class Town
    {
        public Town()
        {
            this.Teams = new HashSet<Team>();
            this.Players = new HashSet<Player>();
        }

        public int TownId { get; set; }

        public string Name { get; set; } = null!;

        public int CountryId { get; set; }

        public Country Country { get; set; } = null!;

        public ICollection<Team> Teams { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
