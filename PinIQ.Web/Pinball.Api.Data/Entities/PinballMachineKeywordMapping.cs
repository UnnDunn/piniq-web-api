using System;

namespace Pinball.Api.Data.Entities
{
    public class PinballMachineKeywordMapping
    {
        public Guid KeywordId { get; set; }
        public PinballMachineKeyword Keyword { get; set; }
        public string MachineId { get; set; }
        public PinballMachine Machine { get; set; }
    }
}
