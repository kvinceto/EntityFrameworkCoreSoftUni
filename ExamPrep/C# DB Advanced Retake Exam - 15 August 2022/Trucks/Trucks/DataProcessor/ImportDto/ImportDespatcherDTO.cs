namespace Trucks.DataProcessor.ImportDto
{
    using System.Xml.Serialization;

    [XmlType("Despatcher")]
    public class ImportDespatcherDTO
    {
        [XmlElement("Name")]
        public string? Name { get; set; }

        [XmlElement("Position")]
        public string Position { get; set; } = null!;

        [XmlArray("Trucks")]
        public ImportTruckDTO[] Trucks { get; set; } = null!;
    }
}
