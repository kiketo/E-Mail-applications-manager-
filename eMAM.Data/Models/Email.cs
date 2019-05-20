using System;
using System.Collections.Generic;
using System.Text;

namespace eMAM.Data.Models
{
    public class Email
    {
        public int Id { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }

        public string Body { get; set; }


    }
}
