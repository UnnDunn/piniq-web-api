using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pinball.Web.Models;

namespace Pinball.Web.Services
{
    public interface ITableCatalogManager
    {
        Task<int> CreateCatalogAsync(PinballTableCatalog snapshot);
        Task UpdateCatalogAsync(PinballTableCatalog snapshot);
        Task<PinballTableCatalog> GetCatalogAsync(int id);
        Task<IList<PinballTableCatalog>> GetCatalogsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task DeleteCatalogAsync(int id);
        Task PublishCatalogAsync(int id, string publisherId);
    }
}
