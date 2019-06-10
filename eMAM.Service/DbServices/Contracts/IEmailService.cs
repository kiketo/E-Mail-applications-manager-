using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices.Contracts
{
    public interface IEmailService
    {
        Task<Email> GetEmailByGmailIdAsync(string id);

        Task<int> GetDbEmailsCountAsync();

        Task<bool> IsEmailInDbAsync(string messageId);

        Task<Email> AddEmailAsync(DateTime dateReceived, List<Attachment> attachments, string gmailIdNumber, Sender sender, Status status, string subject);

        Task AddBodyToMailAsync(Email mail, string body, Status newStatus);

        IQueryable<Email> ReadAllMailsFromDb();

        Task<Email> GetEmailByIdAsync(int id);

        Task<Email> UpdateStatusAsync(Email newEmail, Status newStatus);
    }
}
