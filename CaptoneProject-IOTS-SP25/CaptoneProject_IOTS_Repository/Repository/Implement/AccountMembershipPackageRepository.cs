using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class AccountMembershipPackageRepository : RepositoryBase<AccountMembershipPackage>
    {
        public AccountMembershipPackage? GetByUserId(int userId)
        {
            return _dbSet
                .Include(item => item.MembershipPackageNavigation)
                .SingleOrDefault(item => item.UserId == userId);
        }
    }
}
