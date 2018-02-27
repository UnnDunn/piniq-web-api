using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Pinball.Web.Models;
using Pinball.Web.Services;

namespace Pinball.Web.Pages.Admin.Tables
{
    public class CatalogModel : PageModel
    {
        private readonly ITableCatalogManager _catalogManager;

        public CatalogModel(ITableCatalogManager catalogManager)
        {
            _catalogManager = catalogManager;
        }

        public PinballTableCatalog TableCatalog { get; set; }

        public IList<PinballTable> PinballTables { get; set; }

        public IList<Abbreviation> Abbreviations { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await _catalogManager.GetCatalogAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            TableCatalog = result;

            PinballTables = JsonConvert.DeserializeObject<IList<PinballTable>>(result.JsonTableData);

            Abbreviations = JsonConvert.DeserializeObject<IList<Abbreviation>>(result.JsonAbbreviationData);

            return Page();
        }
    }
}