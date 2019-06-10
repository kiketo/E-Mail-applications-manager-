using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Models
{
    public class ManagerViewModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public User curUser { get; set; }
    }
}
