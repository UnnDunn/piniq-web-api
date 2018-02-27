using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pinball.Web.Data;
using Pinball.Web.Models;

namespace Pinball.Web.Services
{
    public class TableCatalogManager : ITableCatalogManager
    {
        private ApplicationDbContext _context;

        public TableCatalogManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCatalogAsync(PinballTableCatalog snapshot)
        {
            var newCatalogEntity = _context.PinballTableCatalog.Add(snapshot);
            await _context.SaveChangesAsync();

            return newCatalogEntity.Entity.ID;
        }

        public async Task UpdateCatalogAsync(PinballTableCatalog snapshot)
        {
            var existingCatalog = _context.PinballTableCatalog.Update(snapshot);

            await _context.SaveChangesAsync();
        }

        public async Task<PinballTableCatalog> GetCatalogAsync(int id)
        {
            var catalog = await _context.PinballTableCatalog.AsNoTracking().Include(c => c.Uploader).Include(c => c.Publisher).SingleOrDefaultAsync(c => c.ID == id);
            return catalog;
        }

        public async Task<IList<PinballTableCatalog>> GetCatalogsAsync(DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            fromDate = fromDate ?? DateTime.MinValue;
            toDate = toDate ?? DateTime.UtcNow;
            var catalogQuery = _context.PinballTableCatalog
                .Where(p => p.UploadDate >= fromDate)
                .Where(p => p.UploadDate <= toDate);

            var catalogs = await catalogQuery.ToListAsync();
            return catalogs;
        }

        public async Task DeleteCatalogAsync(int id)
        {
            var entity = await _context.PinballTableCatalog.FindAsync(id);
            if (entity == null) throw new InvalidOperationException("Catalog not found");
            _context.PinballTableCatalog.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task PublishCatalogAsync(int id, string publisherId)
        {
            var catalog = await _context.PinballTableCatalog.FindAsync(id);
            if (catalog == null) throw new InvalidOperationException("Catalog not found");

            catalog.PublisherID = publisherId;
            catalog.PublishDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}