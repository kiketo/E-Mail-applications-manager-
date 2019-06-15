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

        public string UserName { get; set; }

        public string OldStatus { get; set; } //TODO

        public string NewStatus { get; set; } //TODO
    }
}
