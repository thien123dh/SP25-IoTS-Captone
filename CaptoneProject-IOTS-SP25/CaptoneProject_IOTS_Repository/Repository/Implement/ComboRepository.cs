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
    public class ComboRepository : RepositoryBase<Combo>
    {

        public Combo? GetById(int id)
        {
            return _dbSet.Include(i => i.StoreNavigation).SingleOrDefault(i => i.Id == id);
        }

        public Combo? GetByApplicationSerialNumber(string applicationNumber)
        {
            return _dbSet.
                FirstOrDefault(item => item.ApplicationSerialNumber == applicationNumber);
        }

        public async Task<List<Combo>> GetByCreatorIdAsync(int creatorId)
        {
            return await _dbSet
                .Where(combo => combo.CreatedBy == creatorId)
                .ToListAsync();
        }
    }
}
