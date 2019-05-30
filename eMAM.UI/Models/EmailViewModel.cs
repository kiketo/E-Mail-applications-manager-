﻿using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Models
{
    public class EmailViewModel
    {
        public int Id { get; set; }

        public string GmailIdNumber { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }

        //TODO: to be encrypted
        public string Body { get; set; }

        public int SenderId { get; set; }
        public Sender Sender { get; set; }

        public DateTime DateReceived { get; set; }

        public string Subject { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

        public bool AreAttachments { get; set; }

        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime InitialRegistrationInSystemOn { get; set; }

        public DateTime SetInCurrentStatusOn { get; set; }

        public DateTime SetInTerminalStatusOn { get; set; }

        public string OpenedById { get; set; }

        public User OpenedBy { get; set; }

        public string ClosedById { get; set; }

        public User ClosedBy { get; set; }

        public ICollection<EmailViewModel> SearchResults { get; set; } = new List<EmailViewModel>();

        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
    }
}