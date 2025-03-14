using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IGHTKService
    {
        Task<List<Province>> SyncProvincesAsync();
        Task<List<District>> SyncDistrictsAsync(int provinceId);
        Task<List<Ward>> SyncWardsAsync(int districtId);
        Task<bool> CreateShipmentAsync(int orderId);
    }
}
