namespace Boardgames.DataProcessor.ExportDto
{
    using Newtonsoft.Json;

    public class ExportSellerDTO
    {
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;

        [JsonProperty("Website")]
        public string Website { get; set; } = null!;

        [JsonProperty("Boardgames")]
        public ExportBoardgameDTO[] Boardgames { get; set; } = null!;
    }
}
