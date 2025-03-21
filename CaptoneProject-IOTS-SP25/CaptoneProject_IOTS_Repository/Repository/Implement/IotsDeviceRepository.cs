using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class IotsDeviceRepository : RepositoryBase<IotsDevice>
    {
        public IotsDevice? GetById(int id)
        {
            return _dbSet
                .Include(item => item.DeviceSpecifications)
                .ThenInclude(dS => dS.DeviceSpecificationsItems)
                .Include(item => item.Category)
                .Include(item => item.StoreNavigation)
                .SingleOrDefault(item => item.Id == id);
        }
        
        public IotsDevice? GetByApplicationSerialNumber(string applicationSerialNumber)
        {
            return _dbSet
                .Include(item => item.Category)
                .Include(item => item.StoreNavigation)
                .SingleOrDefault(item => item.ApplicationSerialNumber == applicationSerialNumber);
        }

        public async Task<List<IotsDevice>> GetByCreatorIdAsync(int creatorId)
        {
            return await _dbSet
                .Where(device => device.CreatedBy == creatorId)
                .ToListAsync();
        }
    }
}
