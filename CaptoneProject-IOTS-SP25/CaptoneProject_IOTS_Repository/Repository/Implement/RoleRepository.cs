using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class RoleRepository : RepositoryBase<Role>
    {
        public IEnumerable<Role> GetActiveRoles()
        {
            return GetAll().Where(i => i.IsActive == 1);
        }
    }
}
