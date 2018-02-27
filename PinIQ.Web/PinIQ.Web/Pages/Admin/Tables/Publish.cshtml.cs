using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Pinball.Web.Data;
using Pinball.Web.Models;
using Pinball.Web.Services;

namespace Pinball.Web.Pages.Admin.Tables
{
    public class PublishModel : PageModel
    {
        private readonly ITableCatalogManager _catalogManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public PinballTableCatalog Catalog { get; set; }

        public IList<PinballTable> PinballTables { get; set; }

        public PublishModel(ITableCatalogManager catalogManager, UserManager<ApplicationUser> userManager)
        {
            _catalogManager = catalogManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await _catalogManager.GetCatalogAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            Catalog = result;

            PinballTables = JsonConvert.DeserializeObject<IList<PinballTable>>(result.JsonTableData).OrderByDescending(t => t.ReleaseDate).Take(10).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user == null) throw new InvalidOperationException();

            try
            {
                await _catalogManager.PublishCatalogAsync(id, user.Id);

                return RedirectToPage("./Catalog", new {id});
            }
            catch (InvalidOperationException ex)
            {
                return NotFound();
            }
        }
    }
}