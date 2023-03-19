namespace CarDealer.DTOs.Import
{
    using System.Xml.Serialization;

    [XmlType("Car")]
    public class ImportCarsDTO
    {
        [XmlElement("make")]
        public string? Make { get; set; }

        [XmlElement("model")]
        public string? Model { get; set; }

        [XmlElement("traveledDistance")]
        public long? TravelledDistance { get; set; }

        [XmlArray("parts")]
        public ImportPartIdDTO[] Parts { get; set; } = null!;
    }
}
