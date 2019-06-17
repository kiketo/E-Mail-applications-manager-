using Crypteron.CipherObject;
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
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext context;

        public CustomerService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Customer> CreateNewCustomerAsync(string egn, string phoneNumber)
        {
            var customer = new Customer()
            {
                
                CustomerEGN = egn,
                CustomerPhoneNumber = phoneNumber,
                Emails = new List<Email>()
            };
            customer.Seal();
            await this.context.Customers.AddAsync(customer);
            await this.context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> GetCustomerByEGNAsync(string egn)
        {
            var customer = await this.context.Customers
                                            .Include(c=>c.Emails)
                                            .FirstOrDefaultAsync(c => c.CustomerEGN == egn)
                                            .Unseal();

            return customer;
        }

        public async Task UpdateAsync(Customer customer)
        {
            customer.Seal();
            this.context.Attach(customer).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }
    }
}
