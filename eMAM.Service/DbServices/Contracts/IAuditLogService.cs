using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices.Contracts
{
    public interface IAuditLogService
    {
        Task Log(string userName, string actionType, Status newStatus, Status oldStatus);
    }
}
