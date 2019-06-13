using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Areas.SuperAdmin.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public User User { get; set; }

        public string Email { get; set; }

        public ICollection<string> Roles { get; set; }

        public IReadOnlyCollection<UserViewModel> UsersList { get; set; }
    }
}
