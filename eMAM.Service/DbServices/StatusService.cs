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
    public class StatusService:IStatusService
    {
        private readonly ApplicationDbContext applicationDbContext;

        public StatusService(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }

        public Task<Status> GetInitialStatusAsync()
        {
            var status = this.applicationDbContext.Statuses.FirstOrDefaultAsync(s => s.Text == "Not Reviewed");
            return status;
        }
        public async Task<Status> GetStatusAsync(string textStatus)
        {
            
            return await this.applicationDbContext.Statuses
                .FirstOrDefaultAsync(s => s.Text == textStatus);
        }

    }
}
