using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class GHTKService : IGHTKService
    {
        private readonly HttpClient _httpClient;
        private readonly UnitOfWork _unitOfWork;

        public GHTKService(HttpClient httpClient, UnitOfWork unitOfWork)
        {
            this._httpClient = httpClient;
            this._unitOfWork ??= unitOfWork;
        }

        public async Task<List<District>> SyncDistrictsAsync(int provinceId)
        {
            try
            {
                var url = $"https://provinces.open-api.vn/api/p/{provinceId}?depth=2";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    return new List<District>();
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var provinceData = JsonConvert.DeserializeObject<ApiProvince>(responseData);

                return provinceData?.districts?.Select(d => new District
                {
                    Id = d.code,
                    Name = d.name,
                    ProvinceId = provinceId
                }).ToList() ?? new List<District>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing districts for province {provinceId}: {ex.Message}");
                return new List<District>();
            }
        }

        public async Task<List<Province>> SyncProvincesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://provinces.open-api.vn/api/p/");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    return new List<Province>();
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var provinces = JsonConvert.DeserializeObject<List<ApiProvince>>(responseData);

                return provinces?.Select(p => new Province
                {
                    Id = p.code,
                    Name = p.name
                }).ToList() ?? new List<Province>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing provinces: {ex.Message}");
                return new List<Province>();
            }
        }

        public async Task<List<Ward>> SyncWardsAsync(int districtId)
        {
            try
            {
                var url = $"https://provinces.open-api.vn/api/d/{districtId}?depth=2";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    return new List<Ward>();
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var districtData = JsonConvert.DeserializeObject<ApiDistrict>(responseData);

                return districtData?.wards?.Select(w => new Ward
                {
                    Id = w.code,
                    Name = w.name,
                    DistrictId = districtId
                }).ToList() ?? new List<Ward>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing wards for district {districtId}: {ex.Message}");
                return new List<Ward>();
            }
        }

    }
}
