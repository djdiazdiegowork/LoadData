using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PMIWeb.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the PMIWebUser class
    public class PMIWebUser : IdentityUser
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageRealName { get; set; }
        public string ImageCustomName { get; set; }
    }
}
