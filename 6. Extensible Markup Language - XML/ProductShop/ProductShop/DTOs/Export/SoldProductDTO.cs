namespace ProductShop.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("SoldProducts")]
    public class SoldProductDTO
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public ExportSoldProductDTO[] Products { get; set; } = null!;
    }
}
