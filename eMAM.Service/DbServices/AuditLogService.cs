using eMAM.Data;
using eMAM.Data.Models;
using eMAM.Service.DbServices.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task Log(string userName, string actionType, Status newStatus, Status oldStatus  )
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            var auditLog = new AuditLog()
            {
                User = user,
                ActionType = actionType,
                NewStatus = newStatus,
                OldStatus = oldStatus,
                TimeStamp = DateTime.Now
            };
            await this.context.AuditLogs.AddAsync(auditLog);
            await this.context.SaveChangesAsync();
        }


    }
}
