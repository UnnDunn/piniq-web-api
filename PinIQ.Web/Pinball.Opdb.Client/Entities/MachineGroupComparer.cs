using System.Collections.Generic;

namespace Pinball.OpdbClient.Entities
{
    public class MachineGroupComparer : IEqualityComparer<MachineGroup>
    {
        public bool Equals(MachineGroup x, MachineGroup y)
        {
            return x.OpdbId.GroupString.Equals(y.OpdbId.GroupString);
        }

        public int GetHashCode(MachineGroup obj)
        {
            return obj.OpdbId.GroupString.GetHashCode();
        }
    }
}
