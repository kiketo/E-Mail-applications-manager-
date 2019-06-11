using eMAM.Data.Models;
using eMAM.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Mappers
{
    public class EmailViewModelMapper : IViewModelMapper<Email, EmailViewModel>
    {
        public EmailViewModel MapFrom(Email entity)
        => new EmailViewModel
        {
            Id=entity.Id,
            Sender=entity.Sender,
            Subject=entity.Subject,
            Body=entity.Body,
            GmailIdNumber=entity.GmailIdNumber,
            Attachments=entity.Attachments,
            ClosedBy=entity.ClosedBy,
            ClosedById=entity.ClosedById,
            Customer=entity.Customer,
            CustomerId=entity.CustomerId,
            DateReceived=entity.DateReceived,
            InitialRegistrationInSystemOn=entity.InitialRegistrationInSystemOn,
            OpenedBy=entity.OpenedBy,
            OpenedById=entity.OpenedById,
            SenderId=entity.SenderId,
            SetInCurrentStatusOn=entity.SetInCurrentStatusOn,
            SetInTerminalStatusOn=entity.SetInTerminalStatusOn,
            Status=entity.Status,
            StatusId=entity.StatusId,
            AreAttachments=entity.Attachments.Any(),
            PreviewedBy=entity.PreviewedBy,
            PreviewedById=entity.PreviewedById,
            WorkingBy=entity.WorkingBy,
            WorkingById=entity.WorkingById,
            WorkInProcess=entity.WorkInProcess
            
        };
    }
}
