using System;

namespace Pinball.Api.Data.Entities
{
    public interface ITimedEntity
    {
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}
