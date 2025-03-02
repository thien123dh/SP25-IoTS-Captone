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
    public class OrderDetailRepository : RepositoryBase<OrderItem>
    {
        public async Task<List<OrderItem>> getAllOrderItem(int Id)
        {
            var result = await _dbSet.ToListAsync();
            return result;
        }

        public async Task<List<OrderItem>> GetByOrderItemByOrderIdAsync(int orderId)
        {
            return await _dbSet.Where(o => o.OrderId == orderId).ToListAsync();
        }

        public IQueryable<OrderItem> GetQueryable(int Id)
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<OrderItem> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
