using System.Collections.Generic;

namespace eMAM.Data.Models
{
    public class Status
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public ICollection<Email> Emails { get; set; }
    }
}