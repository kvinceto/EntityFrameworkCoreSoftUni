namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Text.RegularExpressions;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            ImportDespatcherDTO[] despatchersDtos = XmlHelper.Deserialize<ImportDespatcherDTO[]>(xmlString, "Despatchers");

            StringBuilder result = new StringBuilder();
            Regex regex = new Regex(@"([A-Z]{2}[\d]{4}[A-Z]{2})");

            foreach (var despatherDto in despatchersDtos)
            {
                List<Truck> validTrucks = new List<Truck>();

                if (despatherDto == null)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                if (string.IsNullOrEmpty(despatherDto.Name))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                if (despatherDto.Name.Length < 2 || despatherDto.Name.Length > 40)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Despatcher validDespatcher = new Despatcher()
                {
                    Name = despatherDto.Name,
                    Position = despatherDto.Position
                };

                foreach (var truckDto in despatherDto.Trucks)
                {
                    if (string.IsNullOrEmpty(truckDto.RegistrationNumber) ||
                        string.IsNullOrEmpty(truckDto.VinNumber) ||
                        !truckDto.TankCapacity.HasValue ||
                        !truckDto.CargoCapacity.HasValue ||
                        !truckDto.CategoryType.HasValue ||
                        !truckDto.MakeType.HasValue)
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!regex.IsMatch(truckDto.RegistrationNumber) ||
                        truckDto.VinNumber.Length != 17 ||
                        truckDto.TankCapacity.Value < 950 ||
                        truckDto.TankCapacity.Value > 1420 ||
                        truckDto.CargoCapacity.Value < 5000 ||
                        truckDto.CargoCapacity.Value > 29000 ||
                        truckDto.MakeType.Value < 0 || truckDto.MakeType.Value > 4 ||
                        truckDto.CategoryType.Value < 0 || truckDto.CategoryType.Value > 3)
                    {
                        result.AppendLine(ErrorMessage);
                    }

                    Truck valudTruck = new Truck()
                    {
                        RegistrationNumber = truckDto.RegistrationNumber,
                        VinNumber = truckDto.VinNumber,
                        CargoCapacity = truckDto.CargoCapacity.Value,
                        TankCapacity = truckDto.TankCapacity.Value,
                        CategoryType = (CategoryType)truckDto.CategoryType.Value,
                        MakeType = (MakeType)truckDto.MakeType.Value,
                        Despatcher = validDespatcher,
                    };

                    validTrucks.Add(valudTruck);
                }


                context.Despatchers.Add(validDespatcher);
                context.SaveChanges();
                context.Trucks.AddRange(validTrucks);
                context.SaveChanges();

                result.AppendLine(string.Format(SuccessfullyImportedDespatcher, validDespatcher.Name, validTrucks.Count));
            }

            return result.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            List<ImportClientDTO> clientsDtos =
              JsonConvert.DeserializeObject<List<ImportClientDTO>>(jsonString);

            HashSet<int> validTrucksIds = context.Trucks
                .AsNoTracking()
                .Select(t => t.Id).ToHashSet();

            StringBuilder result = new StringBuilder();

            foreach (var dto in clientsDtos)
            {
                if (string.IsNullOrEmpty(dto.Name) ||
                    string.IsNullOrEmpty(dto.Nationality) ||
                    string.IsNullOrEmpty(dto.Type))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                if (dto.Name.Length < 3 || dto.Name.Length > 40 ||
                    dto.Nationality.Length < 2 || dto.Nationality.Length > 40 ||
                    dto.Type == "usual")
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Client client = new Client()
                {
                    Name = dto.Name,
                    Nationality = dto.Nationality,
                    Type = dto.Type
                };

                List<ClientTruck> validClientTrucks = new List<ClientTruck>();
                foreach (var id in dto.Trucks.Distinct())
                {
                    if (!validTrucksIds.Contains(id))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    ClientTruck clientTruck = new ClientTruck()
                    {
                        Client = client,
                        TruckId = id
                    };

                    validClientTrucks.Add(clientTruck);
                }

                context.Clients.Add(client);
                context.SaveChanges();
                context.ClientsTrucks.AddRange(validClientTrucks);
                context.SaveChanges();

                result.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, validClientTrucks.Count));
            }

            return result.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}