using Crypteron.CipherObject;
using eMAM.Data;
using eMAM.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices
{
    public class SenderService: ISenderService
    {
        private readonly ApplicationDbContext context;

        public SenderService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<Sender> GetSenderAsync(string senderMail, string senderName)
        {
            return this.context.Senders
                        .FirstOrDefaultAsync(s => s.SenderEmail == senderMail && s.SenderName == senderName)
                        .Unseal();
        }

        public async Task<Sender> AddSenderAsync(string senderMail, string senderName)
        {
            Sender newSender = new Sender
            {
                SenderEmail = senderMail,
                SenderName = senderName
            };
            newSender.Seal();
            await this.context.Senders.AddAsync(newSender);
            await this.context.SaveChangesAsync();

            return newSender.Unseal();
        }
    }
}
