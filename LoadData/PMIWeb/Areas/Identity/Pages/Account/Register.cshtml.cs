using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PMIWeb.Areas.Identity.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PMIWeb.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class RegisterModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<PMIWebUser> _signInManager;
        private readonly UserManager<PMIWebUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IHostingEnvironment _environment;

        public RegisterModel(
            IHttpContextAccessor httpContextAccessor,
            UserManager<PMIWebUser> userManager,
            SignInManager<PMIWebUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager, IHostingEnvironment environment)

        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _environment = environment;

            Roles = _httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin") ?
                _roleManager.Roles.Select(r => new SelectRole { RoleName = r.Name }).ToList() :
                _roleManager.Roles.Where(r => r.Name != "SuperAdmin").Select(r => new SelectRole { RoleName = r.Name }).ToList();
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

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres y máximo {1}.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
            [Display(Name = "Confirmar contraseña")]
            public string ConfirmPassword { get; set; }
            public IFormFile Image { get; set; }
        }

        public class SelectRole
        {
            public bool IsSelected { get; set; }
            public string RoleName { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            //var (resultSaveImage, customName, realName) =
            //    await Utils.Utils.SaveImage(Input.Image, _environment.WebRootPath);
            //if (resultSaveImage)
            //{
            var user = new PMIWebUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Name = Input.Name,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                //ImageCustomName = customName,
                //ImageRealName = realName,
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                //// Creando los roles asociados al usuario
                foreach (var selectRole in Roles.Where(sr => sr.IsSelected))
                {
                    await _userManager.AddToRoleAsync(user, selectRole.RoleName);
                }

                _logger.LogInformation("El usuario creó una nueva cuenta con contraseña.");

                return LocalRedirect("/Identity/Admin");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            //}

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
