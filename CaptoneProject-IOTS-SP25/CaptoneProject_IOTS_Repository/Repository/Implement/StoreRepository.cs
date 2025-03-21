using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class StoreRepository : RepositoryBase<Store>
    {
        public Store? GetById(int id)
        {
           return _dbSet.Include(s => s.StoreAttachmentsNavigation)
                .SingleOrDefault(s => s.Id == id);
        }

        public Store? GetByUserId(int userId)
        {
            return _dbSet.Include(s => s.StoreAttachmentsNavigation)
                .FirstOrDefault(s => s.OwnerId == userId);
        }

        public IQueryable<Store> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<Store> GetProductByIdAsync(int storeId)
        {
            return await _context.Stores
                .Include(s => s.Owner)
                .FirstOrDefaultAsync(s => s.Id == storeId);
        }
    }
}
