using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            string pathSuppliers = Path.GetFullPath(@"..\..\..\Datasets\suppliers.json");
            string pathParts = Path.GetFullPath(@"..\..\..\Datasets\parts.json");
            string pathCars = Path.GetFullPath(@"..\..\..\Datasets\cars.json");
            string pathCustomers = Path.GetFullPath(@"..\..\..\Datasets\customers.json");
            string pathSales = Path.GetFullPath(@"..\..\..\Datasets\sales.json");

            //string inputJson = File.ReadAllText(pathSales);

            Console.WriteLine(GetTotalSalesByCustomer(context));
        }

        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            if (suppliers == null)
            {
                throw new NullReferenceException();
            }

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson);


            List<Part> partsList = parts
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                .ToList();

            context.Parts.AddRange(partsList);
            context.SaveChanges();

            return $"Successfully imported {partsList.Count}.";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDto = JsonConvert.DeserializeObject<List<CarsDTO>>(inputJson);

            List<Car> cars = new List<Car>();
            List<PartCar> parts = new List<PartCar>();

            foreach (var carDto in carsDto!)
            {
                Car car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance
                };

                cars.Add(car);

                foreach (var id in carDto.PartsId)
                {
                    PartCar partCar = new PartCar()
                    {
                        Car = car,
                        PartId = id
                    };

                    parts.Add(partCar);
                }
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";

        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers!);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales!);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new CustomerDTO()
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })

                .ToArray();

            string jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return jsonString;
        }

        //15. Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance
                })
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return jsonString;
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Include(s => s.Parts)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .OrderBy(s => s.Id)
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return jsonString;
        }

        //17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TraveledDistance = c.TravelledDistance
                    },
                    parts = c.PartsCars
                    .Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = $"{p.Part.Price:f2}",
                    })
                    .ToArray()
                })
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return jsonString;
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customerSales = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    salePrices = c.Sales.SelectMany(x => x.Car.PartsCars.Select(x => x.Part.Price))
                })
                .ToArray();

            var totalSalesByCustomer = customerSales.Select(t => new
            {
                t.fullName,
                t.boughtCars,
                spentMoney = t.salePrices.Sum()
            })
            .OrderByDescending(t => t.spentMoney)
            .ThenByDescending(t => t.boughtCars)
            .ToArray();

            return JsonConvert.SerializeObject(totalSalesByCustomer, Formatting.Indented);
        }
    }
}