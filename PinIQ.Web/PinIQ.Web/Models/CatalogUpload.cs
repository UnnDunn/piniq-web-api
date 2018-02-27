using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pinball.Web.Models
{
    public class CatalogUpload
    {
        [Display(Name = "Snapshot Name"), Required, StringLength(25)]
        public string Name { get; set; }
        [Display(Name = "Pinball Table HTML"), Required]
        public IFormFile PinballTableHtml { get; set; }

        [Display(Name = "Abbreviation HTML"), Required]
        public IFormFile AbbreviationHtml { get; set; }
    }
}