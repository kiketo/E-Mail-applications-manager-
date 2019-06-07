﻿using eMAM.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eMAM.Service.UserServices.Contracts
{
    public interface IUserService
    {
        Task<List<User>> GetManagersAsync();

        Task<User> GetUserByIdAsync(string id);
    }
}
