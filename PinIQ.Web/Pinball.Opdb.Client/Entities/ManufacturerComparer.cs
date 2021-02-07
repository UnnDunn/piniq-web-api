using System.Collections.Generic;

namespace Pinball.OpdbClient.Entities
{
    public class ManufacturerComparer : IEqualityComparer<Manufacturer?>
    {
        public bool Equals(Manufacturer? x, Manufacturer? y)
        {
            return x?.ManufacturerId == y?.ManufacturerId;
        }

        public int GetHashCode(Manufacturer? obj)
        {
            return obj?.ManufacturerId ?? 0;
        }
    }
}
