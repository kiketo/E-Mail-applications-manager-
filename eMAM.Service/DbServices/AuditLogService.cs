using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext context;

        public AuditLogService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Log(string user, string actionType, string gmailId, string newStatus, string oldStatus  )
        {
            
            var auditLog = new AuditLog()
            {
                UserName = user,
                GmailId = gmailId,
                ActionType = actionType,
                NewStatus = newStatus,
                OldStatus = oldStatus,
                TimeStamp = DateTime.Now
            };
            await this.context.AuditLogs.AddAsync(auditLog);
            await this.context.SaveChangesAsync();
            return;
        }

        public IQueryable<AuditLog> AllLogs()
        {
            var logs = this.context.AuditLogs;

            return logs;
        }


    }
}
