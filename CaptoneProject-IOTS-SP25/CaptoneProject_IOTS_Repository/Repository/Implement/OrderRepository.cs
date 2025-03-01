using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
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
    public class OrderRepository : RepositoryBase<Orders>
    {
        public async Task<List<Orders>> getAllOrder(int Id)
        {
            var result = await _dbSet.ToListAsync();
            return result;
        }

        public async Task<List<Orders>> GetByUserIdAsync(int userId)
        {
            return await _dbSet.Where(o => o.OrderBy == userId).ToListAsync();
        }
    }
}
