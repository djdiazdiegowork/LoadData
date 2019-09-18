using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PMIWeb.Areas.Identity.Data;
using System;
using System.Collections.Generic;

namespace PMIWeb.Utils
{
    public static class SeedData
    {
        public static void InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<PMIWebUser>>();

            var initialUsers = new[] {"superadmin@mail.com", "admin@mail.com"};
            var roleSuperAdmin = "SuperAdmin";
            var initialRoles = new[]
                {roleSuperAdmin, PMIWebIdentityRoles.Admin.ToString("G"), PMIWebIdentityRoles.Buscador.ToString("G")};

            //// Creando los usuarios iniciales
            var userSuperAdmin = CreateIfNotExist(userManager, initialUsers[0], "SuperAdminPMI2019*");
            var userAdmin =  CreateIfNotExist(userManager, initialUsers[1], "AdminPMI2019*");

            //// Creando los roles iniciales
            EnsureRoles(roleManager, initialRoles);
            

            //// Asignando los roles a los usuario iniciales
            userManager.AddToRoleAsync(userSuperAdmin, roleSuperAdmin).Wait();
            userManager.AddToRoleAsync(userAdmin, PMIWebIdentityRoles.Admin.ToString("G")).Wait();
        }

        /// <summary>
        /// Metodo para crear los usuarios iniciales del sistema.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private static PMIWebUser CreateIfNotExist(UserManager<PMIWebUser> userManager, string email, string password)
        {
            var userTask = userManager.FindByEmailAsync(email);
            var user = userTask.Result;
            if (user == null)
            {
                user = new PMIWebUser {UserName = email, Email = email};
                var resultUser = userManager.CreateAsync(user, password);
            }
            return user;
        }

        /// <summary>
        /// Metodo para crear los roles iniciales del sistema
        /// </summary>
        /// <param name="roleManager"></param>
        /// <returns></returns>
        private static void  EnsureRoles(RoleManager<IdentityRole> roleManager, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleCheckTask = roleManager.RoleExistsAsync(role);
                var roleCheck = roleCheckTask.Result;
                if (!roleCheck)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }
        }
    }
}
