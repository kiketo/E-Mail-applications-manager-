using System;
using System.Collections.Generic;
using System.Text;

namespace eMAM.Data.Models
{
    public class Customer
    {
        public int Id { get; set; }

        //TODO: to be encrypted
        public string CustomerEGN { get; set; }

        //TODO: to be encrypted
        public string CustomerPhoneNumber { get; set; }

        public ICollection<Email> Emails { get; set; }
    }
}
