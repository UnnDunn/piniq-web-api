using System;
using System.ComponentModel.DataAnnotations;
using Pinball.Web.Data;

namespace Pinball.Web.Models
{
    public class PinballTableCatalog
    {
        public int ID { get; set; }

        [Display(Name = "Raw Table Data"), DataType(DataType.Upload)]
        public string RawTableData { get; set; }
        [Display(Name = "Raw Table Data Size (bytes)"), DisplayFormat(DataFormatString = "{0:N0}")]
        public long RawTableDataSize { get; set; }
        [Display(Name = "Json Table Data")]
        public string JsonTableData { get; set; }

        [Display(Name = "Raw Abbreviation Data"), DataType(DataType.Upload)]
        public string RawAbbreviationData { get; set; }
        [Display(Name = "Raw Abbreviation Data Size (bytes)"), DisplayFormat(DataFormatString = "{0:N0}")]
        public long RawAbbreviationDataSize { get; set; }
        [Display(Name = "Json Abbreviation Data")]
        public string JsonAbbreviationData { get; set; }

        [Display(Name = "Uploader")]
        public string UploaderID { get; set; }
        public ApplicationUser Uploader { get; set; }
        [Display(Name = "Upload Date"), DataType(DataType.DateTime)]
        public DateTime UploadDate { get; set; }

        [Display(Name = "Publisher")]
        public string PublisherID { get; set; }
        public ApplicationUser Publisher { get; set; }
        [Display(Name = "Publish Date"), DataType(DataType.DateTime)]
        public DateTime? PublishDate { get; set; }
    }
}