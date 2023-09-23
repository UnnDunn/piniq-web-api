using System;
using System.Collections.Generic;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineFeature
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<PinballMachineFeatureMapping> Mappings { get; set; } =
            new List<PinballMachineFeatureMapping>();
    }
}
