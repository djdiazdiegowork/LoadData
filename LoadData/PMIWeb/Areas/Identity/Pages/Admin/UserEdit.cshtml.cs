using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMIWeb.Areas.Identity.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PMIWeb.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class UserEditModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<PMIWebUser> _signInManager;
        private readonly UserManager<PMIWebUser> _userManager;
        //private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        [BindProperty]
        public string UserId { get; set; }
        private IHostingEnvironment _environment;
        public string WebRootFolder { get; set; }


        public UserEditModel(
            IHttpContextAccessor httpContextAccessor,
            UserManager<PMIWebUser> userManager,
            SignInManager<PMIWebUser> signInManager,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager, IHostingEnvironment environment)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _environment = environment;
            WebRootFolder = _environment.WebRootPath;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public List<SelectRole> Roles { get; set; }

        public class InputModel
        {

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [Display(Name = "Nombre")]
            public string Name { get; set; }

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [Display(Name = "Primer Apellido")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [Display(Name = "Segundo Apellido")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [EmailAddress(ErrorMessage = "La dirección de {0} no es válida")]
            [Display(Name = "Correo")]
            public string Email { get; set; }

            //[Required]
            //[StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} y maximo {1} cantidad de caracteres.", MinimumLength = 6)]
            //[DataType(DataType.Password)]
            //[Display(Name = "Contraseña")]
            //public string Password { get; set; }

            //[DataType(DataType.Password)]
            //[Compare("Password", ErrorMessage = "La contraseña y la confirmación deben ser la misma.")]
            //[Display(Name = "Confirmar contraseña")]
            //public string ConfirmPassword { get; set; }
            public IFormFile Image { get; set; }
            public string ImagePath { get; set; }
        }

        public class SelectRole
        {
            public bool IsSelected { get; set; }
            public string RoleName { get; set; }
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

            Input = new InputModel();
            UserId = userId;
            Input.Name = user.Name;
            Input.FirstName = user.FirstName;
            Input.LastName = user.LastName;
            Input.Email = user.Email;

            Input.ImagePath = string.Empty;
            if (!string.IsNullOrEmpty(user.ImageRealName))
                Input.ImagePath = user.ImageRealName;

            Roles = _httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin") ?
                _roleManager.Roles.
                    Select(r => new SelectRole
                    {
                        RoleName = r.Name
                    }).ToList() : 
                _roleManager.Roles.Where(r => r.Name != "SuperAdmin").
                    Select(r => new SelectRole
                    {
                        RoleName = r.Name
                    }).ToList();

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in Roles)
                role.IsSelected = userRoles.Contains(role.RoleName);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null) return Page();
            //var (resultSaveImage, customName, realName) = await Utils.Utils.SaveImage(Input.Image, _environment.WebRootPath, user.ImageRealName);

            //if (!resultSaveImage) return Page();
            user.Email = Input.Email;
            user.Name = Input.Name;
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            //user.ImageCustomName = customName;
            //user.ImageRealName = realName;

            await _userManager.UpdateAsync(user);

            var userRoles = _userManager.GetRolesAsync(user).Result.ToList();
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            //// Creando los roles asociados al usuario
            //foreach (var selectRole in Roles.Where(sr => sr.IsSelected))
            //{
            //    await _userManager.AddToRoleAsync(user, selectRole.RoleName);
            //}

            await _userManager.AddToRolesAsync(user, Roles.Where(sr => sr.IsSelected).Select(sr => sr.RoleName));

            //_logger.LogInformation("User created a new account with password.");

            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var callbackUrl = Url.Page(
            //    "/Account/ConfirmEmail",
            //    pageHandler: null,
            //    values: new { userId = user.Id, code = code },
            //    protocol: Request.Scheme);

            //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
            //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //await _signInManager.SignInAsync(user, isPersistent: false);
            //return LocalRedirect(returnUrl);
            return RedirectToPage("./Index");

            // If we got this far, something failed, redisplay form
        }
    }
}