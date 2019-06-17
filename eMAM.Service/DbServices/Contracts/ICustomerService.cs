using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.DbServices.Contracts
{
    public interface ICustomerService
    {
        Task<Customer> CreateNewCustomerAsync(string egn, string phoneNumber);

        Task<Customer> GetCustomerByEGNAsync(string egn);

        Task UpdateAsync(Customer customer);
    }
}
