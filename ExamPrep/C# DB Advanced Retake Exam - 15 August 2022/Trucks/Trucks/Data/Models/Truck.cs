namespace Trucks.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using Trucks.Data.Models.Enums;

    public class Truck
    {
        public Truck()
        {
            this.ClientsTrucks = new HashSet<ClientTruck>();
        }

        public int Id { get; set; }

        public string? RegistrationNumber { get; set; }

        public string VinNumber { get; set; } = null!;

        [Range(950, 1420)]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        public CategoryType CategoryType { get; set; }

        public MakeType MakeType { get; set; }

        public int DespatcherId { get; set; }

        public Despatcher Despatcher { get; set; } = null!;

        public ICollection<ClientTruck> ClientsTrucks { get; set; }
    }
}
