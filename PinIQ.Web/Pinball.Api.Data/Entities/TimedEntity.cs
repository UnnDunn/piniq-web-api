using System;

namespace Pinball.Api.Data.Entities
{
    public abstract class TimedEntity : ITimedEntity
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public TimedEntity()
        {
            Created = DateTime.UtcNow;
        }
    }
}
