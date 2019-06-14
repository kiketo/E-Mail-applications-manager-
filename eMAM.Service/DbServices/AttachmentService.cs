using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices
{
    public class AttachmentService:IAttachmentService
    {
        private readonly ApplicationDbContext context;

        public AttachmentService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        //(attachment, newMail)
        public async Task<Attachment> AddAttachmentAsync(Attachment attachment)
        {
            await this.context.Attachments.AddAsync(attachment);
            await this.context.SaveChangesAsync();

            return attachment;
        }
    }
}
