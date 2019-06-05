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

        public async Task<Attachment> AddAttachmentAsync(string fileName, double fileSizeInB)
        {
            Attachment newAttachment = new Attachment
            {
                FileName = fileName,
                FileSizeInMb = fileSizeInB / (1024 * 1024)
            };

            await this.context.Attachments.AddAsync(newAttachment);
            await this.context.SaveChangesAsync();

            return newAttachment;
        }
    }
}
