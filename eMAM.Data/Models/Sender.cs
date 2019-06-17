using Crypteron;
using System.Collections.Generic;

namespace eMAM.Data.Models
{
    public class Sender
    {
        public int Id { get; set; }

        [Secure]
        public string SenderEmail { get; set; }

        [Secure]
        public string SenderName { get; set; }

        public ICollection<Email> Emails { get; set; }
    }
}
