using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
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
        Task<List<Address>> SyncAddressAsync(int wardId);
        Task<bool> CreateShipmentAsync(int orderId);
        Task<List<ShippingFeeResponse>> GetShippingFeeAsync(ShippingFeeRequest requestModel);
        Task<byte[]> PrintLabelAsync(string trackingOrder);
        Task<string> GetTrackingOrderAsync(int orderId);
    }
}

