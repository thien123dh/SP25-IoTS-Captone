using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class WalletRepository : RepositoryBase<Wallet>
    {
        public Wallet? GetByUserId(int userId)
        {
            return _dbSet
                .Include("UserNavigation")
                .SingleOrDefault(item => item.UserId == userId);
        }
    }
}
