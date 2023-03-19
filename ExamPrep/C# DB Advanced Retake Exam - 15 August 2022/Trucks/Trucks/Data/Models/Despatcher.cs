namespace Trucks.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Despatcher
    {
        public Despatcher()
        {
            this.Trucks = new HashSet<Truck>();
        }

        public int Id { get; set; }

        [MinLength(2)]
        public string Name { get; set; } = null!;

        public string Position { get; set; } = null!;

        public ICollection<Truck> Trucks { get; set; }
    }
}
