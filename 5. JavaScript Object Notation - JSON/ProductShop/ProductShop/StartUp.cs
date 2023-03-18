namespace ProductShop
{
    using Newtonsoft.Json;

    using Data;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using ProductShop.DTOs.Export;

    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();
            string filePathUsers = Path.GetFullPath(@"..\..\..\Datasets\users.json");
            string filePathProducts = Path.GetFullPath(@"..\..\..\Datasets\products.json");
            string filePathCategories = Path.GetFullPath(@"..\..\..\Datasets\categories.json");
            string filePathCategoriesProducts = Path.GetFullPath(@"..\..\..\Datasets\categories-products.json");

            string inputJson = File.ReadAllText(filePathCategoriesProducts);


            Console.WriteLine(GetUsersWithProducts(context));

        }

        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var jsonUsers = JsonConvert.DeserializeObject<List<User>>(inputJson);

            if (jsonUsers == null)
            {
                throw new NullReferenceException();
            }

            context.Users.AddRange(jsonUsers);
            context.SaveChanges();

            return $"Successfully imported {jsonUsers.Count}";
        }

        //Query 2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var jsonProducts = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            if (jsonProducts == null)
            {
                throw new NullReferenceException();
            }

            context.Products.AddRange(jsonProducts);
            context.SaveChanges();

            return $"Successfully imported {jsonProducts.Count}";
        }

        //Query 3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var jsonCategories = JsonConvert.DeserializeObject<List<Category>>(inputJson);

            if (jsonCategories == null)
            {
                throw new NullReferenceException();
            }

            int counter = 0;
            foreach (var caregorie in jsonCategories)
            {
                if (caregorie.Name != null)
                {
                    context.Categories.Add(caregorie);
                    counter++;
                }
            }
            context.SaveChanges();

            return $"Successfully imported {counter}";
        }

        //Query 4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var jsonCategorieProducts =
                JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            if (jsonCategorieProducts == null)
            {
                throw new NullReferenceException();
            }

            context.CategoriesProducts.AddRange(jsonCategorieProducts);
            context.SaveChanges();

            return $"Successfully imported {jsonCategorieProducts.Count}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .AsNoTracking()
                .Include(p => p.Seller)
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ProductDTO()
                {
                    Name = p.Name,
                    Price = p.Price,
                    SelerName = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .ToList();

            string jsonString = JsonConvert.SerializeObject(products, Formatting.Indented);
            return jsonString;
        }

        //06. Export Sold Products 
        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold.Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer!.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                })
                .ToArray();

            return JsonConvert.SerializeObject(soldProducts, Formatting.Indented);
        }

        //Query 7. Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriesByProductsCount = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = Math.Round((double)c.CategoriesProducts.Average(p => p.Product.Price), 2),
                    totalRevenue = Math.Round((double)c.CategoriesProducts.Sum(p => p.Product.Price), 2)
                })
                .ToArray();

            return JsonConvert.SerializeObject(categoriesByProductsCount, Formatting.Indented);
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold
                                         .Where(p => p.Buyer != null)
                                         .Count())
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Count(),
                        products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price
                            })
                    }
                })
                .ToArray();

            var usersInfo = new
            {
                usersCount = usersWithProducts.Count(),
                users = usersWithProducts
            };

            string output = JsonConvert.SerializeObject(usersInfo, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            });

            return output;
        }


    }
}