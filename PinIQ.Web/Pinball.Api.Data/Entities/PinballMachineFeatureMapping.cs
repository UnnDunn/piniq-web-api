using System;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineFeatureMapping
    {
        public Guid FeatureId { get; set; }
        public PinballMachineFeature Feature { get; set; }
        public string MachineId { get; set; }
        public PinballMachine Machine { get; set; }
    }
}
