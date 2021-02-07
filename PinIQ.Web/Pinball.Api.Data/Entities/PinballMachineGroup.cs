using System.Collections.Generic;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<PinballMachine> Machines { get; set; }
    }
}
