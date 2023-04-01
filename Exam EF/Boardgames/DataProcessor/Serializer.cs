namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var dtos = context.Creators
                .AsNoTracking()
                .Include(c => c.Boardgames)
                .Where(c => c.Boardgames.Count > 0)
                .Select(c => new ExportCreatorDTO()
                {
                    Name = c.FirstName + " " + c.LastName,
                    Count = c.Boardgames.Count,
                    Boardgames = c.Boardgames
                   .Select(b => new ExportBoardgameXMLDTO()
                   {
                       Name = b.Name,
                       BoardgameYearPublished = b.YearPublished
                   })
                   .OrderBy(b => b.Name)
                   .ToArray()
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.Name)
                .ToList();

            return xmlHelper.Serialize<List<ExportCreatorDTO>>(dtos, "Creators");
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var dtos = context.Sellers
                .AsNoTracking()
                .Include(s => s.BoardgamesSellers)
                .ThenInclude(s => s.Boardgame)
                .Where(s => s.BoardgamesSellers.Any(b => b.Boardgame.YearPublished >= year && b.Boardgame.Rating <= rating))
                .Select(s => new ExportSellerDTO()
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                    .Where(b => b.Boardgame.YearPublished >= year && b.Boardgame.Rating <= rating)
                    .Select(b => new ExportBoardgameDTO()
                    {
                        Name = b.Boardgame.Name,
                        Rating = b.Boardgame.Rating,
                        Mechanics = b.Boardgame.Mechanics,
                        Category = b.Boardgame.CategoryType.ToString()
                    })
                   .OrderByDescending(b => b.Rating)
                   .ThenBy(b => b.Name)
                   .ToArray()
                })
                .OrderByDescending(s => s.Boardgames.Length)
                .ThenBy(s => s.Name)
                .Take(5)
                .ToList();

            return JsonConvert.SerializeObject(dtos, Formatting.Indented);
        }
    }
}