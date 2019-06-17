using Crypteron;
using System;
using System.Collections.Generic;
using System.Text;

namespace eMAM.Data.Models
{
    public class Email
    {
        public int Id { get; set; }

        public string GmailIdNumber { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }

        [Secure]
        public string Body { get; set; }

        public int SenderId { get; set; }

        public Sender Sender { get; set; }

        public DateTime DateReceived { get; set; }

        [Secure]
        public string Subject { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

        public int? CustomerId { get; set; }

        public Customer Customer { get; set; }

        public bool MissingApplication { get; set; }

        public DateTime InitialRegistrationInSystemOn { get; set; }

        public DateTime SetInCurrentStatusOn { get; set; }

        public DateTime SetInTerminalStatusOn { get; set; }

        public string OpenedById { get; set; }

        public User OpenedBy { get; set; }

        public string ClosedById { get; set; }

        public User ClosedBy { get; set; }

        public bool WorkInProcess { get; set; }

        public string WorkingById { get; set; }

        public User WorkingBy { get; set; }

        public string PreviewedById { get; set; }

        public User PreviewedBy { get; set; }
    }
}
