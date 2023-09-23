using System;
using System.Collections.Generic;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineKeyword
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<PinballMachineKeywordMapping> Mappings { get; set; } = null!;
    }
}
