using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using CaptoneProject_IOTS_Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class LocationRepository : RepositoryBase<Province>
    {
        public async Task<bool> ProvinceExistsAsync(int provinceId)
        {
            return await ExistsAsync(p => p.Id == provinceId);
        }
    }
}
