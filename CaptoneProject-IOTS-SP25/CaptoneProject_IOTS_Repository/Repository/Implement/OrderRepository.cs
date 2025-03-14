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

        public IQueryable<Orders> GetQueryable(int orderId)
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<Orders> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<List<Orders>> GetOrdersByStoreIdAsync(int storeId)
        {
            return await _dbSet
                .Where(o => o.OrderItems.Any(oi => oi.SellerId == storeId))
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.IotsDevice) 
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)  
                .ToListAsync();
        }

        public async Task<Orders> GetOrderByIdAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.OrderItems) // Load OrderItems để tránh null
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
