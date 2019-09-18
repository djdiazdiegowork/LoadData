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
    public class UserDeleteModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<PMIWebUser> _userManager;
        private IHostingEnvironment _environment;

        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public PMIWebUser CurrentUser { get; set; }
        public IList<string> Roles { get; set; }

        public UserDeleteModel(
            IHttpContextAccessor httpContextAccessor,
            UserManager<PMIWebUser> userManager, 
            IHostingEnvironment environment
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _environment = environment;
            Roles = new List<string>();
        }

        public async Task<IActionResult> OnGet(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") && !_httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin"))
            {
                return RedirectPermanent("/Identity/Account/AccessDenied");
            }

            CurrentUser = user;
            UserId = user.Id;
            Roles = await _userManager.GetRolesAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            var user = _userManager.FindByIdAsync(UserId).Result;
            if (user != null)
            {
                Utils.Utils.DeleteFile(_environment.WebRootPath, user.ImageRealName);
                await _userManager.DeleteAsync(user);
            }

            return LocalRedirect(returnUrl);
        }
    }
}