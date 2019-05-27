using System.Collections.Generic;

namespace eMAM.Data.Models
{
    public class Sender
    {
        public int Id { get; set; }

        //TODO: to be encrypted
        public string SenderEmail { get; set; }

        //TODO: to be encrypted
        public string SenderName { get; set; }

        public ICollection<Email> Emails { get; set; }
    }
}
