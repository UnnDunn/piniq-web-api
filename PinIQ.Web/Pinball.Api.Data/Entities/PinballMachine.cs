using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachine
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? CommonName { get; set; }
        public int? IpdbId { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public int? ManufacturerId { get; set; }
        public PinballMachineManufacturer? Manufacturer { get; set; }
        public short PlayerCount { get; set; }
        public virtual ICollection<PinballMachineKeywordMapping> KeywordMappings { get; set; } = null!;
        public List<string>? Keywords => KeywordMappings?.Select(k => k.Keyword.Name).ToList();
        public string? TypeId { get; set; }
        public PinballMachineType? Type { get; set; }
        public virtual ICollection<PinballMachineFeatureMapping> FeatureMappings { get; set; } = null!;
        public List<string>? Features => FeatureMappings?.Select(f => f.Feature.Name).ToList();
        public string? MachineGroupId { get; set; }
        public PinballMachineGroup? MachineGroup { get; set; }
    }
}
