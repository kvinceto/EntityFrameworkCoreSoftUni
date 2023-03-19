namespace ProductShop
{
    using Microsoft.EntityFrameworkCore;
    using ProductShop.Data;
    using ProductShop.DTOs.Export;
    using ProductShop.DTOs.Import;
    using ProductShop.Models;
    using System.Text;
    using System.Xml.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();

            // context.Database.EnsureDeleted();
            // context.Database.EnsureCreated();
            // string filePathUsers = Path.GetFullPath(@"..\..\..\Datasets\users.xml");
            // Console.WriteLine(ImportUsers(context, File.ReadAllText(filePathUsers)));

            // string filePathProducts = Path.GetFullPath(@"..\..\..\Datasets\products.xml");
            // Console.WriteLine(ImportProducts(context, File.ReadAllText(filePathProducts)));

            // string filePathCategories = Path.GetFullPath(@"..\..\..\Datasets\categories.xml");
            // Console.WriteLine(ImportCategories(context, File.ReadAllText (filePathCategories)));

            // string filePathCategoriesProducts = Path.GetFullPath(@"..\..\..\Datasets\categories-products.xml");
            // Console.WriteLine(
            // ImportCategoryProducts(context, File.ReadAllText(filePathCategoriesProducts)));

            // Console.WriteLine(GetProductsInRange(context));
            // Console.WriteLine(GetSoldProducts(context));
            // Console.WriteLine(GetCategoriesByProductsCount(context));
            // Console.WriteLine(GetUsersWithProducts(context));
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

            T dtos = (T)serializer.Deserialize(reader);
            return dtos;
        }

        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var users = Deserialize<ImportUserDTO[]>(inputXml, "Users");

            List<User> validUsers = new List<User>();
            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.LastName))
                {
                    continue;
                }

                User validUser = new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Age = user.Age
                };

                validUsers.Add(validUser);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var products = Deserialize<ImportProductDTO[]>(inputXml, "Products");

            List<Product> validProducts = new List<Product>();
            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Name) ||
                    !product.Price.HasValue ||
                    !product.SellerId.HasValue)
                {
                    continue;
                }

                Product validProduct = new Product()
                {
                    Name = product.Name,
                    Price = product.Price.Value,
                    SellerId = product.SellerId.Value,
                    BuyerId = product.BuyerId
                };

                validProducts.Add(validProduct);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var categories = Deserialize<CategorieDTO[]>(inputXml, "Categories");

            List<Category> validCategories = new List<Category>();
            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.Name))
                {
                    continue;
                }

                Category validCategory = new Category()
                {
                    Name = category.Name
                };

                validCategories.Add(validCategory);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var array = Deserialize<ImportCategoryProduct[]>(inputXml, "CategoryProducts");

            HashSet<int> validProductIds = context.Products
                .AsNoTracking()
                .Select(p => p.Id).ToHashSet();
            HashSet<int> validCategoyIds = context.Categories
                .AsNoTracking()
                .Select(c => c.Id).ToHashSet();

            List<CategoryProduct> validCategoryProducts = new List<CategoryProduct>();
            foreach (var item in array)
            {
                if (!item.ProductId.HasValue || !item.CategoryId.HasValue)
                {
                    continue;
                }
                if (!validProductIds.Contains(item.ProductId.Value) ||
                    !validCategoyIds.Contains(item.CategoryId.Value))
                {
                    continue;
                }

                CategoryProduct validCategoryProduct = new CategoryProduct()
                {
                    CategoryId = item.CategoryId.Value,
                    ProductId = item.ProductId.Value
                };

                validCategoryProducts.Add(validCategoryProduct);
            }

            context.CategoryProducts.AddRange(validCategoryProducts);
            context.SaveChanges();

            return $"Successfully imported {validCategoryProducts.Count}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Include(p => p.Buyer)
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExportProductDTO()
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerName = string.Join(" ", p.Buyer!.FirstName, p.Buyer.LastName)
                })
                .Take(10)
                .ToArray();


            return Serializer<ExportProductDTO[]>(products, "Products");
        }

        //06. Export Sold Products


        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Include(c => c.CategoryProducts)
                .ThenInclude(c => c.Product)
                .Select(c => new ExportCategoryByProductsDTO()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            return Serializer<ExportCategoryByProductsDTO[]>(categories, "Categories");
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            UserDTO[] users = context.Users
                .Include(u => u.ProductsSold)
                .Where(u => u.ProductsSold.Count > 0)
                             .Select(u => new UserDTO()
                             {
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 Age = u.Age,
                                 SoldProducts = new SoldProductDTO()
                                 {
                                     Count = u.ProductsSold.Count,
                                     Products = u.ProductsSold
                                     .Select(p => new ExportSoldProductDTO()
                                     {
                                         Name = p.Name,
                                         Price = p.Price
                                     })
                                     .ToArray()
                                 }
                             })
                .ToArray();

            UserDTOFull user = new UserDTOFull()
            {
                Count = users.Length,
                Users = users
            };

            return Serializer<UserDTOFull>(user, "Users");

        }
    }
}