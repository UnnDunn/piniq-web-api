using System.Collections.Generic;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineManufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? FullName { get; set; }
        public virtual ICollection<PinballMachine> Machines { get; set; } = new List<PinballMachine>();
    }
}
