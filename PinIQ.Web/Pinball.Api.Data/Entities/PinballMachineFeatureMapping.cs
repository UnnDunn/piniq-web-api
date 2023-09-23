using System;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineFeatureMapping
    {
        public Guid FeatureId { get; set; }
        public PinballMachineFeature Feature { get; set; } = null!;
        public string MachineId { get; set; } = null!;
        public PinballMachine Machine { get; set; } = null!;
    }
}
