using System.Collections.Generic;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineGroup
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? ShortName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<PinballMachine> Machines { get; set; } = null!;
    }
}
