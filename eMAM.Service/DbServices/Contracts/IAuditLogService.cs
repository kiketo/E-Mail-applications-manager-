using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices.Contracts
{
    public interface IAuditLogService
    {
        Task Log(string user, string actionType, string gmailId, string newStatus, string oldStatus);

        IQueryable<AuditLog> AllLogs();
    }
}
