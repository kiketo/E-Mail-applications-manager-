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

        Task ValidateEmail(Email mail, string body, Status newStatus, User user);

        IQueryable<Email> ReadAllMailsFromDb(bool isManager, User user);

        Task<Email> GetEmailByIdAsync(int id);

        Task UpdateAsync(Email newEmail);

        Task<Email> WorkInProcessAsync(User user, string messageId);

        Task<Email> WorkNotInProcessAsync(string messageId);

        Task<string> GetEmailBodyAsync(string mailId);

        
    }
}
