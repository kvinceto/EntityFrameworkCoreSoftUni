namespace Trucks.DataProcessor
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Trucks.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var dtos = context.Despatchers
                .Include(d => d.Trucks)
                .AsNoTracking()
                .Where(d => d.Trucks.Count() > 0)
                .Select(d => new ExportDespatcherDTO()
                {
                    DespatcherName = d.Name,
                    TrucksCount = d.Trucks.Count(),
                    Trucks = d.Trucks
                    .Select(t => new ExportTruckDTO()
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString(),
                    })
                    .OrderBy(t => t.RegistrationNumber)
                    .ToArray()
                })
                .OrderByDescending(d => d.TrucksCount)
                .ThenBy(d => d.DespatcherName)
                .ToArray();

            string result = XmlHelper.Serializer<ExportDespatcherDTO[]>(dtos, "Despatchers");
            return result;
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Include(c => c.ClientsTrucks)
                .ThenInclude(c => c.Truck)
                .AsNoTracking()
                .Where(c =>
                      (c.ClientsTrucks
                      .Where(ct => ct.Truck.TankCapacity >= capacity))
                      .Count() > 0)
                .Select(c => new
                {
                    Name = c.Name,
                    Trucks = c.ClientsTrucks
                              .Where(ct => ct.Truck.TankCapacity >= capacity)
                              .Select(t => new
                              {
                                  TruckRegistrationNumber = t.Truck.RegistrationNumber,
                                  VinNumber = t.Truck.VinNumber,
                                  TankCapacity = t.Truck.TankCapacity,
                                  CargoCapacity = t.Truck.CargoCapacity,
                                  CategoryType = t.Truck.CategoryType.ToString(),
                                  MakeType = t.Truck.MakeType.ToString()
                              })
                              .OrderBy(t => t.MakeType)
                              .ThenByDescending(t => t.CargoCapacity)
                              .ToList()
                })
                .OrderByDescending(c => c.Trucks.Count)
                .ThenBy(c => c.Name)
                .Take(10)
                .ToList();

            string jsonString = JsonConvert.SerializeObject(clients, Formatting.Indented);
            return jsonString;
        }
    }
}
