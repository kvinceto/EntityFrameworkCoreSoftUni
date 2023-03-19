namespace Trucks.DataProcessor.ImportDto
{
    using System.Xml;
    using System.Xml.Serialization;

    [XmlType("Truck")]
    public class ImportTruckDTO
    {
        [XmlElement("RegistrationNumber")]
        public string? RegistrationNumber { get; set; }

        [XmlElement("VinNumber")]
        public string? VinNumber { get; set; }

        [XmlElement("TankCapacity")]
        public int? TankCapacity { get; set; }

        [XmlElement("CargoCapacity")]
        public int? CargoCapacity { get; set; }

        [XmlElement("CategoryType")]
        public int? CategoryType { get; set; }

        [XmlElement("MakeType")]
        public int? MakeType { get; set; }
    }
}
