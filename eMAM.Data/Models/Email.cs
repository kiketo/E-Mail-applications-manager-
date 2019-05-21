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

        //TODO: to be encrypted
        public string Body { get; set; }

        //TODO: to be encrypted
        public string SenderEmail { get; set; }

        //TODO: to be encrypted
        public string SenderName { get; set; }

        public DateTime DateReceived { get; set; }

        public string Subject { get; set; }

        public List<string> AttachmentsFileNames { get; set; }

        public List<double> AttachmentsFilesSizeInMb { get; set; }

        //TODO: to be encrypted
        public int? CustomerEGN { get; set; }

        //TODO: to be encrypted
        public string CustomerPhoneNumber { get; set; }

        public DateTime InitialRegistrationInSystemOn { get; set; }

        public DateTime SetInCurrentStatusOn { get; set; }

        public DateTime SetInTerminalStatusOn { get; set; }

        public string OpenedById { get; set; }

        public User OpenedBy { get; set; }

        public string ClosedById { get; set; }

        public User ClosedBy { get; set; }
    }
}
