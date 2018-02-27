using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Pinball.Entities;

namespace Pinball.Web.Models
{
    public class PinballTable : PinballTableEntity
    {
        [Display(Name = "Table Name")]
        public override string Name { get; set; }

        [Display(Name = "Release Date"), DataType(DataType.Date)]
        public override DateTime ReleaseDate { get; set; }

        [Display(Name = "Player Count")]
        public override short PlayerCount { get; set; }

        [Display(Name = "IPDB Url"), DataType(DataType.Url)]
        public override string IpdbUrl { get; set; }

        [JsonIgnore] public override string DisplayName => base.DisplayName;

        [JsonIgnore]
        public IEnumerable<Abbreviation> Abbreviations { get; set; }
    }
}