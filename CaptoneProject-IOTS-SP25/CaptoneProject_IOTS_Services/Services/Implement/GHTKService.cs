using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class GHTKService : IGHTKService
    {
        private readonly HttpClient _httpClient;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public GHTKService(HttpClient httpClient, IConfiguration configuration, UnitOfWork unitOfWork)
        {
            this._httpClient = httpClient;
            this._configuration = configuration;
            this._unitOfWork ??= unitOfWork;
        }

        public async Task<ResponseDTO> CalculateShippingFeeAsync()
        {
            try
            {
                var token = _configuration["GHTK:Token"];
                var baseUrl = "https://services-staging.ghtklab.com";

                // Mã hóa URL để tránh lỗi ký tự đặc biệt
                var pickProvince = HttpUtility.UrlEncode("Thành Phố Hồ Chí Minh");
                var pickDistrict = HttpUtility.UrlEncode("Thành Phố Thủ Đức");
                var province = HttpUtility.UrlEncode("Thành Phố Hồ Chí Minh");
                var district = HttpUtility.UrlEncode("Quận 1");
                var address = HttpUtility.UrlEncode("Phường Bến Nghé");
                var weight = 1000;
                var value = 3000000;
                var deliverOption = "xteam";

                // Đưa tất cả tham số vào URL
                var url = $"{baseUrl}/services/shipment/fee?" +
                          $"pick_province={pickProvince}&pick_district={pickDistrict}&" +
                          $"province={province}&district={district}&address={address}&" +
                          $"weight={weight}&value={value}&deliver_option={deliverOption}";

                // Thêm Token vào Header
                if (!_httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    _httpClient.DefaultRequestHeaders.Add("Token", token);
                }

                // Gửi request
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Response Body: {responseContent}");

                return JsonConvert.DeserializeObject<ResponseDTO>(responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CalculateShippingFeeAsync: {ex.Message}");
                return new ResponseDTO { IsSuccess = false, Message = "Lỗi khi tính phí vận chuyển" };
            }
        }

        public async Task<bool> CreateShipmentAsync(int orderId)
        {
            try
            {
                var token = _configuration["GHTK:Token"];
                var baseUrl = "https://services-staging.ghtklab.com";
                var url = $"{baseUrl}/services/shipment/order/";

                // Lấy order từ database
                var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                    return false;

                var requestData = new
                {
                    products = order.OrderItems.Select(item => new
                    {
                        name = $"SP{item.ProductType}{DateTime.Now}",
                        weight = 0.5,
                        quantity = item.Quantity,
                        price = (int)item.Price,
                        product_code = $"SP{item.Id}{DateTime.Now}"
                    }).ToList(),
                    order = new
                    {
                        id = $"{order.ApplicationSerialNumber}{DateTime.Now}",
                        pick_name = "Don Noi Thanh",
                        pick_address = order.Address,
                        pick_province = "TP Hồ Chí Minh",
                        pick_district = "Thành Phố Thủ Đức",
                        pick_ward = "Phường Long Bình",
                        pick_tel = $"0364463482",
                        name = "GHTK - HCM - Noi Thanh",
                        address = "123 nguyễn chí thanh",
                        province = "TP. Hồ Chí Minh",
                        district = "Quận 1",
                        ward = "Phường Bến Nghé",
                        tel = "0935402099",
                        hamlet = "Khác",
                        is_freeship = 0,
                        pick_money = 0,
                        note = $"{order.Notes}",
                        value = (int)order.TotalPrice,
                    }
                };

                // Serialize JSON bằng Newtonsoft.Json
                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestData,
                    Newtonsoft.Json.Formatting.Indented);

                Console.WriteLine($"Request Payload:\n{jsonPayload}");

                // Kiểm tra xem header đã tồn tại chưa
                if (!_httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    _httpClient.DefaultRequestHeaders.Add("Token", token);
                }

                // Tạo nội dung request với application/json
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Gửi request
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"GHTK API Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    // Cập nhật trạng thái OrderItem thành 2
                    foreach (var item in order.OrderItems)
                    {
                        item.OrderItemStatus = 2;
                        await _unitOfWork.OrderDetailRepository.SaveAsync();
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine($"GHTK API Error: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return false;
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
