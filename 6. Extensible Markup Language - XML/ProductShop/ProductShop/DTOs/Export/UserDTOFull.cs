namespace ProductShop.DTOs.Export
{
    using System.Xml.Serialization;

    public class UserDTOFull
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public UserDTO[] Users { get; set; } = null!;
    }
}
