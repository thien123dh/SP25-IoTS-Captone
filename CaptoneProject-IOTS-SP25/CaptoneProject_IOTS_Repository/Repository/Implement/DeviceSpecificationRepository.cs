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
    public class DeviceSpecificationRepository : RepositoryBase<DeviceSpecification>
    {

        public List<DeviceSpecification>? GetDeviceSpecificationByDeviceId(int deviceId)
        {
            return _dbSet
                .Include(ds => ds.DeviceSpecificationsItems)
                .Where(ds => ds.IotDeviceId == deviceId)
                .ToList();
        }

    }
}
