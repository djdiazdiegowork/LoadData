using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMIWeb.Areas.Identity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMIWeb.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class UserDetailModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<PMIWebUser> _userManager;
        public PMIWebUser CurrentUser { get; set; }
        public IList<string> Roles { get; set; }
        private IHostingEnvironment _environment;
        public string WebRootFolder { get; set; }

        public UserDetailModel(
            IHttpContextAccessor httpContextAccessor,
            UserManager<PMIWebUser> userManager,
            IHostingEnvironment environment
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _environment = environment;
            Roles = new List<string>();
            WebRootFolder = _environment.WebRootPath;
        }

        public async Task<IActionResult> OnGet(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Imposible cargar usario con Id '{userId}'.");
            }

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") && !_httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin"))
            {
                return RedirectPermanent("/Identity/Account/AccessDenied");
            }

            CurrentUser = user;
            Roles = await _userManager.GetRolesAsync(user);
            return Page();
        }
    }
}