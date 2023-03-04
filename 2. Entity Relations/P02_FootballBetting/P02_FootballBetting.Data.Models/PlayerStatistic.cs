namespace P02_FootballBetting.Data.Models
{
    public class PlayerStatistic
    {
        public PlayerStatistic()
        {

        }

        public int GameId { get; set; }

        public Game Game { get; set; } = null!;

        public int PlayerId { get; set; }

        public Player Player { get; set; } = null!;

        public int ScoredGoals { get; set; }

        public int Assists { get; set; }

        public int MinutesPlayed { get; set; }
    }
}
