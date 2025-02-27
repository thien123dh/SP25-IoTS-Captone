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
    public class DeviceComboRepository : RepositoryBase<IotsDevicesCombo>
    {
        public List<IotsDevicesCombo>? GetItemsByComboId(int comboId)
        {
            return _dbSet
                .Include(item => item.IotDeviceNavigation)
                .Where(item => item.ComboId == comboId)?
                .ToList();

        }
    }
}
