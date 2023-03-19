namespace CarDealer
{
    using CarDealer.Data;
    using CarDealer.DTOs.Import;
    using CarDealer.Models;
    using Microsoft.EntityFrameworkCore;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string pathSuppliers = Path.GetFullPath(@"..\..\..\Datasets\suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, File.ReadAllText(pathSuppliers)));

            //string pathParts = Path.GetFullPath(@"..\..\..\Datasets\parts.xml");
            //Console.WriteLine(ImportParts(context, File.ReadAllText(pathParts)));

            //string pathCars = Path.GetFullPath(@"..\..\..\Datasets\cars.xml");
            //Console.WriteLine(ImportCars(context, File.ReadAllText(pathCars)));
        }

        private static string Serializer<T>(T dataTransferObjects, string xmlRootAttributeName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttributeName));

            StringBuilder sb = new StringBuilder();
            using var write = new StringWriter(sb);

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(write, dataTransferObjects, xmlNamespaces);

            return sb.ToString();
        }

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), root);

            using StringReader reader = new StringReader(inputXml);

            T dtos = (T)serializer.Deserialize(reader)!;
            return dtos!;
        }

        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var suppliersDtos = Deserialize<ImportSupplierDTO[]>(inputXml, "Suppliers");

            List<Supplier> valisSuppliers = new List<Supplier>();
            foreach (var supplier in suppliersDtos)
            {
                if (string.IsNullOrEmpty(supplier.Name) || !supplier.IsImporter.HasValue)
                {
                    continue;
                }

                Supplier validSupplier = new Supplier()
                {
                    Name = supplier.Name,
                    IsImporter = supplier.IsImporter.Value
                };

                valisSuppliers.Add(validSupplier);
            }

            context.Suppliers.AddRange(valisSuppliers);
            context.SaveChanges();

            return $"Successfully imported {valisSuppliers.Count}";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            HashSet<int> validSuppliersIds = context.Suppliers
                .Select(s => s.Id)
                .ToHashSet();
            ImportPartDTO[] partsDtos = Deserialize<ImportPartDTO[]>(inputXml, "Parts");

            List<Part> validParts = new List<Part>();
            foreach (var dto in partsDtos)
            {
                if (string.IsNullOrEmpty(dto.Name) || !dto.Price.HasValue ||
                    !dto.Quantity.HasValue || !dto.SupplierId.HasValue)
                {
                    continue;
                }

                if (!validSuppliersIds.Contains(dto.SupplierId.Value))
                {
                    continue;
                }

                Part validPart = new Part()
                {
                    Name = dto.Name,
                    Price = dto.Price.Value,
                    Quantity = dto.Quantity.Value,
                    SupplierId = dto.SupplierId.Value
                };

                validParts.Add(validPart);
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            ImportCarsDTO[] carsDtos = Deserialize<ImportCarsDTO[]>(inputXml, "Cars");

            List<Car> cars = new List<Car>();
            List<PartCar> partCars = new List<PartCar>();
            int[] allPartIds = context.Parts.Select(p => p.Id).ToArray();
            int carId = 1;

            foreach (var dto in carsDtos)
            {
                if (string.IsNullOrEmpty(dto.Make) || string.IsNullOrEmpty(dto.Model) ||
                    !dto.TravelledDistance.HasValue)
                {
                    continue;
                }

                Car car = new Car()
                {
                    Make = dto.Make!,
                    Model = dto.Model!,
                    TravelledDistance = dto.TravelledDistance!.Value
                };

                cars.Add(car);

                foreach (int partId in dto.Parts
                    .Where(p => allPartIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        CarId = carId,
                        PartId = partId
                    };
                    partCars.Add(partCar);
                }
                carId++;
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(partCars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }


    }
}