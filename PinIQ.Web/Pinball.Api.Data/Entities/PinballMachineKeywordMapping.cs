using System;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineKeywordMapping
    {
        public Guid KeywordId { get; set; }
        public PinballMachineKeyword Keyword { get; set; } = null!;
        public string MachineId { get; set; } = null!;
        public PinballMachine Machine { get; set; } = null!;
    }
}
