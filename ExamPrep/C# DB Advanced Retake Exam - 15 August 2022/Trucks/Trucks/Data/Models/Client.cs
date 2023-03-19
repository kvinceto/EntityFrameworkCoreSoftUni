namespace Trucks.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        public Client()
        {
            this.ClientsTrucks = new HashSet<ClientTruck>();
        }

        public int Id { get; set; }

        [MinLength(3)]
        public string Name { get; set; } = null!;

        [MinLength(2)]
        public string Nationality { get; set; } = null!;

        public string Type { get; set; } = null!;

        public ICollection<ClientTruck> ClientsTrucks { get; set; }
    }
}
