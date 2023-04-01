namespace Boardgames.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Creator")]
    public class ExportCreatorDTO
    {
        [XmlAttribute("BoardgamesCount")]
        public int Count { get; set; }

        [XmlElement("CreatorName")]
        public string Name { get; set; } = null!;

        [XmlArray("Boardgames")]
        public ExportBoardgameXMLDTO[] Boardgames { get; set; } = null!;
    }
}
