namespace P02_FootballBetting.Data.Models
{
    public class Player
    {
        public Player()
        {
            this.PlayersStatistics = new HashSet<PlayerStatistic>();
        }

        public int PlayerId { get; set; }

        public string Name { get; set; } = null!;

        public int SquadNumber { get; set; }

        public int TownId { get; set; }

        public Town Town { get; set; } = null!;

        public int TeamId { get; set; }

        public Team Team { get; set; } = null!;

        public int PositionId { get; set; }

        public Position Position { get; set; } = null!;

        public bool IsInjured { get; set; }

        public ICollection<PlayerStatistic> PlayersStatistics { get; set; }
    }
}
