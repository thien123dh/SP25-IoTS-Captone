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
    public class LabRepository : RepositoryBase<Lab>
    {
        public Lab? GetById(int id)
        {
            return _dbSet
                .Include(item => item.ComboNavigation)
                .ThenInclude(c => c.StoreNavigation)
                .Include(item => item.CreatedByNavigation)
                .SingleOrDefault(item => item.Id == id);
        }
    }
}
