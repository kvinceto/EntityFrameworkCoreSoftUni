namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var dtos = xmlHelper
                .DeserializeCollection<ImportCreatorDTO>(xmlString, "Creators");

            StringBuilder sb = new StringBuilder();

            List<Creator> validCreators = new List<Creator>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var creator = new Creator()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };

                foreach (var game in dto.Boardgames)
                {
                    if (!IsValid(game))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    creator.Boardgames.Add(new Boardgame()
                    {
                        Name = game.Name,
                        Rating = game.Rating,
                        YearPublished = game.YearPublished,
                        CategoryType = (CategoryType)game.CategoryType,
                        Mechanics = game.Mechanics,
                        Creator = creator
                    });
                }

                validCreators.Add(creator);

                sb.AppendLine(string.Format(SuccessfullyImportedCreator, creator.FirstName, creator.LastName, creator.Boardgames.Count));
            }

            context.Creators.AddRange(validCreators);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            HashSet<int> validIds = context.Boardgames
                .AsNoTracking()
                .Select(b => b.Id)
                .ToHashSet();

            var dtos = JsonConvert.DeserializeObject<ImportSellerDTO[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            List<Seller> validSellers = new List<Seller>();

            foreach (var dto in dtos!)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var seller = new Seller()
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Country = dto.Country,
                    Website = dto.Website
                };

                foreach (int id in dto.Boardgames)
                {
                    if (!validIds.Contains(id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    seller.BoardgamesSellers.Add(new BoardgameSeller()
                    {
                        BoardgameId = id
                    });
                }

                validSellers.Add(seller);

                sb.AppendLine(string.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count));
            }

            context.Sellers.AddRange(validSellers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
