using System;
using System.Collections.Generic;
using System.Text;

namespace eMAM.Data.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string GmailId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ActionType { get; set; } //TODO?????

        public User User { get; set; }

        public Status OldStatus { get; set; } //TODO

        public Status NewStatus { get; set; } //TODO
    }
}
