namespace ProductShop.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("User")]
    public class ExportSoldProductsByUser
    {
        [XmlElement("firstName")]
        public string? FirstName { get; set; }

        [XmlElement("lastName")]
        public string? LastName { get; set; }

        [XmlArray("soldProducts")]
        public ExportSoldProductDTO[] SoldProducts { get; set; } = null!;
    }
}
