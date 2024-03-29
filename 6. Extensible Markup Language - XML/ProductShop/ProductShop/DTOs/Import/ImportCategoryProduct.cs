﻿namespace ProductShop.DTOs.Import
{
    using System.Xml.Serialization;

    [XmlType("CategoryProduct")]
    public class ImportCategoryProduct
    {
        [XmlElement("CategoryId")]
        public int? CategoryId { get; set; }

        [XmlElement("ProductId")]
        public int? ProductId { get; set; }
    }
}
