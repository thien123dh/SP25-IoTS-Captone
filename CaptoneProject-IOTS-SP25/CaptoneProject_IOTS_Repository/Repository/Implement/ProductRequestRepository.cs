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
    public class ProductRequestRepository : RepositoryBase<ProductRequest>
    {
        public async Task<ProductRequest> GetProductRequestById(int id)
        {
            var res = await _dbSet
                .Include(item => item.MaterialGroupNavigation)
                .Include(item => item.LabNavigation)
                .Include(item => item.MaterialGroupNavigation)
                .Include(item => item.CreatedByNavigation)
                .FirstOrDefaultAsync(item => item.Id == id);

            return res;
        }
    }
}
