namespace Trucks.DataProcessor.ImportDto
{
    using Newtonsoft.Json;

    public class ImportClientDTO
    {
        [JsonProperty("Name")]
        public string? Name { get; set; }

        [JsonProperty("Nationality")]
        public string? Nationality { get; set; }

        [JsonProperty("Type")]
        public string? Type { get; set; }

        [JsonProperty("Trucks")]
        public int[] Trucks { get; set; } = null!;
    }
}
