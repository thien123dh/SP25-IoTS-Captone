﻿using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class UserRequestRepository : RepositoryBase<UserRequest>
    {
        public async Task<UserRequest> GetByEmail(string email) 
        {
            var response = await _dbSet
                .Include(ur => ur.StatusNavigation)
                .SingleOrDefaultAsync(ur => ur.Email == email);

            return response;
        } 

    }
}