namespace BookShop
{
    using System.Text;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            Console.WriteLine(RemoveBooks(db));
        }

        //02. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            command = command.ToLower();
            int cmd = 0;
            switch (command)
            {
                case "minor": cmd = 0; break;
                case "teen": cmd = 1; break;
                case "adult": cmd = 2; break;
                default: cmd = 3; break;
            }

            var books = context.Books
                 .Where(b => (int)b.AgeRestriction == cmd)
                 .Select(b => new
                 {
                     b.Title
                 })
                 .OrderBy(b => b.Title)
                 .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //03. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.EditionType.Equals(EditionType.Gold) &&
                            b.Copies < 5000)
                .Select(b => new
                {
                    b.BookId,
                    b.Title
                })
                .OrderBy(b => b.BookId)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        //04. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //05. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate != null && b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    b.BookId,
                    b.Title
                })
                .OrderBy(b => b.BookId)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        //06. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            HashSet<string> categories = input
                .ToLower()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet();

            var books = context.BooksCategories
                .Where(bc => categories.Contains(bc.Category.Name.ToLower()))
                .Select(bc => bc.Book.Title)
                .ToArray();

            Array.Sort(books, (x, y) => String.Compare(x, y));

            string result = string.Join(Environment.NewLine, books);

            return result;
        }

        //07. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            string[] dateParams = date.Split("-", StringSplitOptions.RemoveEmptyEntries);
            DateTime dateTime =
                new DateTime(int.Parse(dateParams[2]),
                             int.Parse(dateParams[1]),
                             int.Parse(dateParams[0]));
            var books = context.Books
                .Where(b => b.ReleaseDate != null)
                .Where(b => b.ReleaseDate < dateTime)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //08. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a =>
                    a.FirstName
                     .Substring((a.FirstName.Length - input.Length), input.Length) == input)
                .Select(a => $"{a.FirstName} {a.LastName}")
                .ToArray()
                .OrderBy(name => name);

            return string.Join(Environment.NewLine, authors);
        }

        //09. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var titles = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(title => title)
                .ToArray();

            return string.Join(Environment.NewLine, titles);
        }

        //10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Include(b => b.Author)
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    FullName = $"{b.Author.FirstName} {b.Author.LastName}"
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FullName})");
            }

            return sb.ToString().TrimEnd();
        }

        //11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int count = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();
            return count;
        }

        //12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var result = context.Authors
                .Include(a => a.Books)
                .OrderByDescending(a => a.Books.Sum(b => b.Copies))
                .Select(a => $"{a.FirstName} {a.LastName} - {a.Books.Sum(b => b.Copies)}")
                .ToArray();

            return string.Join(Environment.NewLine, result);
        }

        //13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categorieProfit = context.Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(bc => bc.Book)
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks
                        .Sum(cb =>
                            (cb.Book.Price * cb.Book.Copies)
                            )
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var entry in categorieProfit)
            {
                sb.AppendLine($"{entry.Name} ${entry.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //14. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var data = context.Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks
                             .Where(cb => cb.Book.ReleaseDate != null)
                             .Select(cb => new
                             {
                                 cb.Book.Title,
                                 cb.Book.ReleaseDate
                             })
                             .OrderByDescending(cb => cb.ReleaseDate)
                             .Take(3)
                             .ToArray()

                })
                .OrderBy(c => c.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var entry in data)
            {
                sb.AppendLine($"--{entry.Name}");
                foreach (var book in entry.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate!.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate != null && b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();
            context.RemoveRange(books);

            context.SaveChanges();

            return books.Length;
        }
    }
}


