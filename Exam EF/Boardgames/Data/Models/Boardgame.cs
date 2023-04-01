namespace Boardgames.Data.Models
{
    using Boardgames.Data.Models.Enums;

    public class Boardgame
    {
        public Boardgame()
        {
            this.BoardgamesSellers = new HashSet<BoardgameSeller>();
        }
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public double Rating { get; set; }

        public int YearPublished { get; set; }

        public CategoryType CategoryType { get; set; }

        public string Mechanics { get; set; } = null!;

        public int CreatorId { get; set; }

        public virtual Creator Creator { get; set; } = null!;

        public virtual ICollection<BoardgameSeller> BoardgamesSellers { get; set; }
    }
}
