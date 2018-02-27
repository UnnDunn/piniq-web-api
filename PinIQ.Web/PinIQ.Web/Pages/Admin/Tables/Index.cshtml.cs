using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pinball.IpdbParser;
using Pinball.Web.Data;
using Pinball.Web.Models;
using Pinball.Web.Services;
using Pinball.Web.Utilities;

namespace Pinball.Web.Pages.Admin.Tables
{
    public class IndexModel : PageModel
    {
        private readonly ITableCatalogManager _catalogManager;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public CatalogUpload CatalogUpload { get; set; }

        public IList<PinballTableCatalog> Catalogs { get; private set; }

        public IndexModel(ITableCatalogManager catalogManager, UserManager<ApplicationUser> userManager)
        {
            _catalogManager = catalogManager;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var result = await _catalogManager.GetCatalogsAsync();

            Catalogs = result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Catalogs = await _catalogManager.GetCatalogsAsync();
                return Page();
            }

            var pinballTables = await FileHelpers.ProcessFormFile(CatalogUpload.PinballTableHtml, ModelState, async (p) => await ParsePinballTables(p));

            var abbreviations = await FileHelpers.ProcessFormFile(CatalogUpload.AbbreviationHtml, ModelState, async (a) => await ParseAbbreviations(a));

            if (!ModelState.IsValid)
            {
                Catalogs = await _catalogManager.GetCatalogsAsync();
                return Page();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var catalog = new PinballTableCatalog
            {
                RawTableData = pinballTables.Item1,
                RawTableDataSize = pinballTables.Item1.Length,
                JsonTableData = pinballTables.Item2,
                RawAbbreviationData = abbreviations.Item1,
                RawAbbreviationDataSize = abbreviations.Item1.Length,
                JsonAbbreviationData = abbreviations.Item2,
                UploaderID = user.Id,
                UploadDate = DateTime.UtcNow
            };

            var id = await _catalogManager.CreateCatalogAsync(catalog);

            return RedirectToPage("./Catalog", new { id });
        }


        private static async Task<IEnumerable<PinballTable>> ParsePinballTables(string tableHtml)
        {
            var tableParser = new PinballTableParser();
            var pinballTables = await tableParser.ParseData(tableHtml);
            var pinballTableEntities = pinballTables.Select(p => new PinballTable
            {
                Name = p.Name,
                Manufacturer = p.Manufacturer,
                PlayerCount = p.PlayerCount,
                IpdbUrl = p.IpdbUrl,
                Id = p.Id,
                SortName = p.Name.SortableTitle(),
                ReleaseDate = p.ReleaseDate
            }).ToList();
            return pinballTableEntities;
        }

        private static async Task<IEnumerable<Abbreviation>> ParseAbbreviations(string abbreviationHtml)
        {
            var abbreviationParser = new AbbreviationParser();
            var pinballAbbreviations = await abbreviationParser.ParseData(abbreviationHtml);
            var abbreviationEntities = pinballAbbreviations
                .Select(a => new Abbreviation { Name = a.Name, TableID = a.TableID }).ToList();
            return abbreviationEntities;
        }
    }
}