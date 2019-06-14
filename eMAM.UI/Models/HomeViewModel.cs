using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Models
{
    public class HomeViewModel
    {

        public ICollection<EmailViewModel> NotReviewed { get; set; }

        public ICollection<EmailViewModel> New { get; set; }

        public ICollection<EmailViewModel> Open { get; set; }

        public ICollection<EmailViewModel> Closed { get; set; }

        public bool UserIsManager { get; set; }
    }
}
