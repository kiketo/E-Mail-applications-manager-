using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace eMAM.Data.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class User : IdentityUser
    {
        public ICollection<Email> OpenedEmails { get; set; }

        public ICollection<Email> ClosedEmails { get; set; }

        public bool Inactive { get; set; }
    }
}
