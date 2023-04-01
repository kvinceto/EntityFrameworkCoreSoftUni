namespace Boardgames.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Boardgame")]
    public class ExportBoardgameXMLDTO
    {
        [XmlElement("BoardgameName")]
        public string Name { get; set; } = null!;

        [XmlElement("BoardgameYearPublished")]
        public int BoardgameYearPublished { get; set; }
    }
}
