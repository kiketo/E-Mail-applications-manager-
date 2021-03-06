﻿using Crypteron.CipherObject;
using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext context;

        public EmailService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Email> GetEmailByGmailIdAsync(string id)
        {
            var mail = await this.context.Emails
                        .Include(x => x.Attachments)
                        .Include(x => x.Customer)
                        .Include(x => x.OpenedBy)
                        .Include(x => x.Sender)
                        .Include(x => x.Status)
                        .FirstOrDefaultAsync(x => x.GmailIdNumber == id);

            if (mail == null)
            {
                throw new ArgumentException($"There is no mail with Gmail ID:{id}");
            }
            mail.Unseal();
            return mail;
        }

        public Task<int> GetDbEmailsCountAsync()
        {
            return this.context.Emails.CountAsync();
        }

        public Task<bool> IsEmailInDbAsync(string messageId)
        {
            return this.context.Emails.AnyAsync(e => e.GmailIdNumber == messageId);
        }

        public async Task<Email> AddEmailAsync(DateTime dateReceived, List<Attachment> attachments, string gmailIdNumber, Sender sender, Status status, string subject)
        {
            Email newEmail = new Email
            {
                Attachments = attachments,
                Status = status,
                Sender = sender,
                DateReceived = dateReceived,
                GmailIdNumber = gmailIdNumber,
                InitialRegistrationInSystemOn = DateTime.Now,
                SetInCurrentStatusOn = DateTime.Now,
                Subject = subject
            };
            newEmail.Seal();
            await this.context.Emails.AddAsync(newEmail);
            await this.context.SaveChangesAsync();
            //newEmail.Unseal();
            return newEmail;
        }

        //public async Task ValidateEmail(Email mail, string body, Status newStatus, User user)
        //{
        //    mail.Body = body;
        //    mail.Status = newStatus;
        //    mail.SetInCurrentStatusOn = DateTime.Now;
        //    mail.WorkInProcess = false;
        //    mail.WorkingBy = null;
        //    mail.WorkingById = null;
        //    mail.PreviewedBy = user;

        //    await this.context.SaveChangesAsync();
        //}

        public IQueryable<Email> ReadAllMailsFromDb(bool isManager, User user)
        {
            IQueryable<Email> allMails;
            if (isManager)
            {
                allMails = this.context.Emails
                                       .Include(e => e.Attachments)
                                       .Include(e => e.Sender)
                                       .Include(e => e.Status)
                                       .Include(e => e.ClosedBy)
                                       .Include(e => e.PreviewedBy)
                                       .Include(e => e.OpenedBy);
            }
            else
            {
                allMails = this.context.Emails
                                           .Where(e => e.WorkInProcess == true && e.WorkingBy == user || e.WorkInProcess == false)
                                           .Include(e => e.Attachments)
                                           .Include(e => e.Sender)
                                           .Include(e => e.Status)
                                           .Include(e => e.ClosedBy)
                                            .Include(e => e.PreviewedBy)
                                           .Include(e => e.OpenedBy);
            }
            allMails.Unseal();
            return allMails;
        }

        public IQueryable<Email> ReadOpenMailsFromDb(bool isManager, User user)
        {
            IQueryable<Email> allMails;
            if (isManager)
            {
                allMails = this.context.Emails
                                       .Where(e => e.Status.Text == "Open")
                                       .Include(e => e.OpenedBy)
                                       .Include(e => e.Attachments)
                                       .Include(e => e.Sender)
                                       .Include(e => e.Status).OrderBy(e => e.SetInCurrentStatusOn);

            }
            else
            {
                allMails = this.context.Emails
                                           .Where(e => e.Status.Text == "Open")
                                           .Where(e => e.WorkInProcess == true && e.WorkingBy == user || e.WorkInProcess == false)
                                           .Include(e => e.Attachments)
                                           .Include(e => e.Sender)
                                           .Include(e => e.Status).OrderBy(e => e.SetInCurrentStatusOn);

            }
            allMails.Unseal();
            return allMails;
        }

        public IQueryable<Email> ReadClosedMailsFromDb(bool isManager, User user)
        {
            IQueryable<Email> allMails;
            if (isManager)
            {
                allMails = this.context.Emails
                                       .Include(e => e.Status)
                                       .Where(e => e.Status.Text == "Aproved" || e.Status.Text == "Rejected")
                                       .Include(e => e.Attachments)
                                        .Include(e => e.ClosedBy)
                                       .Include(e => e.Sender);
            }
            else
            {
                allMails = this.context.Emails
                                           .Include(e => e.Status)
                                           .Where(e => e.Status.Text == "Aproved" || e.Status.Text == "Rejected")
                                           .Where(e => e.ClosedBy == user)
                                           .Include(e => e.Attachments)
                                            .Include(e => e.ClosedBy)
                                           .Include(e => e.Sender);
            }
            allMails.Unseal();
            return allMails;
        }

        public async Task<Email> GetEmailByIdAsync(int id)
        {
            var mail = await this.context.Emails
                            .FirstOrDefaultAsync(e => e.Id == id);
            mail.Unseal();
            return mail;
        }

        public async Task UpdateAsync(Email newEmail)
        {
            newEmail.Seal();
            this.context.Attach(newEmail).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
            newEmail.Unseal();
        }

        public async Task<string> GetEmailBodyAsync(string mailId)
        {
            var mail = await this.context.Emails
                                .FirstOrDefaultAsync(e => e.GmailIdNumber == mailId);
            mail.Unseal();

            return mail.Body;
        }

        public async Task<Email> WorkInProcessAsync(User user, string messageId)
        {
            var mail = await this.GetEmailByGmailIdAsync(messageId);
            mail.WorkInProcess = true;
            mail.WorkingBy = user;
            mail.Seal();
            await this.context.SaveChangesAsync();

            mail.Unseal();
            return mail;
        }

        public async Task<Email> WorkNotInProcessAsync(string messageId)
        {
            var mail = await this.GetEmailByGmailIdAsync(messageId);
            mail.WorkInProcess = false;
            mail.WorkingBy = null;
            mail.Seal();
            await this.context.SaveChangesAsync();

            mail.Unseal();
            return mail;
        }

        public IQueryable<Email> ReadClosedByUserEmailsDb(User user)
        {
            IQueryable<Email> allMails;

            allMails = this.context.Emails
                                       .Where(e => e.ClosedBy == user || e.WorkInProcess == false)
                                       .Where(s => s.Status.Text == "Aproved" || s.Status.Text == "Rejected")
                                       .Include(e => e.Attachments)
                                       .Include(e => e.Sender)
                                       .Include(e => e.Status);
            allMails.Unseal();
            return allMails;
        }
    }
}
