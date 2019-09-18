using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMIWeb.Areas.Identity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMIWeb.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<PMIWebUser> _userManager;
        public List<PMIWebUser> Users { get; set; }

        public IndexModel(
            IHttpContextAccessor httpContextAccessor,
            UserManager<PMIWebUser> userManager
           )
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task OnGet()
        {
            Users = new List<PMIWebUser>(); 
            if (_httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin"))
                foreach (var user in _userManager.Users)
                    Users.Add(user);
            else
                foreach (var user in _userManager.Users)
                    if (!await _userManager.IsInRoleAsync(user, "SuperAdmin")) Users.Add(user);

            //Users = _httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin") ? _userManager.Users
            //    : await _userManager.Users.Where(async  ( u) => await !_userManager.IsInRoleAsync(u, "SuperAdmin"));

        }
    }
}