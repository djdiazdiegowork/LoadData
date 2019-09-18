using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PMIWeb.Areas.Identity.Data;
namespace PMIWeb.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ChangePasswordModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<PMIWebUser> _userManager;
        private readonly SignInManager<PMIWebUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        [BindProperty]
        public string UserId { get; set; }

        public ChangePasswordModel(
            IHttpContextAccessor httpContextAccessor,
            UserManager<PMIWebUser> userManager,
            SignInManager<PMIWebUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            //[Required]
            //[DataType(DataType.Password)]
            //[Display(Name = "Current password")]
            //public string OldPassword { get; set; }

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres y máximo {1}.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nueva contraseña")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirma la contraseña")]
            [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") && !_httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin"))
            {
                return RedirectPermanent("/Identity/Account/AccessDenied");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            UserId = userId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, Input.NewPassword);
            await _userManager.UpdateAsync(user);

            //await _signInManager.RefreshSignInAsync(user);
            //_logger.LogInformation("User changed their password successfully.");
            //StatusMessage = "Your password has been changed.";

            //return RedirectToPage();
            return LocalRedirect(returnUrl);
        }
    }
}
