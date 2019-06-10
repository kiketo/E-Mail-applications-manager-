using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices.Contracts
{
    public interface IStatusService
    {
        Task<Status> GetInitialStatusAsync();

        Task<Status> GetStatusByName(string statusName);

        Task<Status> GetStatusAsync(string textStatus);
    }
}
