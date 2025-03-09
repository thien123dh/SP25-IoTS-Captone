using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using CaptoneProject_IOTS_Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class DistrictRepository : RepositoryBase<District>
    {
        public async Task<bool> DistrictExistsAsync(int districtId)
        {
            return await ExistsAsync(d => d.Id == districtId);
        }
    }
}
