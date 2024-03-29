﻿using Newtonsoft.Json;

namespace CarDealer.DTOs.Export
{
    public class CustomerDTO
    {
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;

        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; } = null!;

        [JsonProperty("IsYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
