namespace ProductShop.DTOs.Export
{
    using Newtonsoft.Json;

    public class ProductDTO
    {
        public ProductDTO()
        {

        }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("seller")]
        public string SelerName { get; set; } = null!;
    }
}
