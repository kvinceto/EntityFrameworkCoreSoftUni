namespace Boardgames.Data.Models
{
    public class BoardgameSeller
    {
        public int BoardgameId { get; set; }

        public virtual Boardgame Boardgame { get; set; } = null!;

        public int SellerId  { get; set; }

        public virtual Seller Seller { get; set; } = null!;
    }
}
