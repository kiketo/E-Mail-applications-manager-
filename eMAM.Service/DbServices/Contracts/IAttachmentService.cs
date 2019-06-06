using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices.Contracts
{
    public interface IAttachmentService
    {
        Task<Attachment> AddAttachmentAsync(Attachment attachment);
    }
}
