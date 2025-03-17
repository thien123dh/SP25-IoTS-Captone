using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
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
using System.Linq;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class GHTKService : IGHTKService
    {
        private readonly HttpClient _httpClient;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IUserServices _userServices;

        public GHTKService(HttpClient httpClient, IConfiguration configuration, UnitOfWork unitOfWork, IUserServices userServices)
        {
            this._httpClient = httpClient;
            this._configuration = configuration;
            this._unitOfWork ??= unitOfWork;
            this._userServices = userServices;
        }

        public async Task<List<ShippingFeeResponse>> GetShippingFeeAsync(ShippingFeeRequest requestModel)
        {
            try
            {
                var loginUser = _userServices.GetLoginUser();
                if (loginUser == null || !await _userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                    return new List<ShippingFeeResponse> { new ShippingFeeResponse { Message = "You don't have permission to access" } };

                var loginUserId = loginUser.Id;
                // Get the list of selected products in the cart
                var selectedItems = await _unitOfWork.CartRepository
                    .GetQueryable((int)loginUserId)
                    .Where(item => item.CreatedBy == loginUserId && item.IsSelected)
                    .Include(item => item.IosDeviceNavigation)
                    .Include(item => item.ComboNavigation)
                    .ToListAsync();

                if (selectedItems == null || !selectedItems.Any())
                {
                    return new List<ShippingFeeResponse> { new ShippingFeeResponse { Message = "The cart is empty or no products have been selected." } };
                }

                ////////////////////////////////////////////////// Information Customer////////////////////////////////////////////////////////////////////////////

                var provincesCustomer = await SyncProvincesAsync();
                var provinceCustomer = provincesCustomer.FirstOrDefault(p => p.Id == requestModel.ProvinceId);
                if (provinceCustomer == null)
                    return new List<ShippingFeeResponse> { new ShippingFeeResponse { Message = "Invalid Province ID" } };
                var ProvinceNameCustomer = provinceCustomer?.Name ?? "Not found";

                var districtsCustomer = await SyncDistrictsAsync(requestModel.ProvinceId);
                var districtCustomer = districtsCustomer.FirstOrDefault(d => d.Id == requestModel.DistrictId);
                if (districtCustomer == null)
                    return new List<ShippingFeeResponse> { new ShippingFeeResponse { Message = "Invalid Province ID" } };
                var DistrictNameCustomer = districtCustomer?.Name ?? "Not found";

                var wardsCustomer = await SyncWardsAsync(requestModel.DistrictId);
                var wardCustomer = wardsCustomer.FirstOrDefault(w => w.Id == requestModel.WardId);
                if (wardCustomer == null)
                    return new List<ShippingFeeResponse> { new ShippingFeeResponse { Message = "Invalid Province ID" } };
                var WardNameCustomer = wardCustomer?.Name ?? "Not found";

                var list_addressCustomer = await SyncAddressAsync(requestModel.WardId);
                var addressNameCustomer = list_addressCustomer.FirstOrDefault(w => w.Id == requestModel.AddressId);
                if (addressNameCustomer == null)
                    return new List<ShippingFeeResponse> { new ShippingFeeResponse { Message = "Invalid Province ID" } };
                var AddressNameCustomer = addressNameCustomer?.Name ?? "Not found";

                var groupedShops = selectedItems.GroupBy(item => item.SellerId)
                    .Select(group => new
                    {
                        ShopOwner = group.Key, // ID của shop
                        TotalWeight = group.Sum(item =>
                        ((item.IosDeviceNavigation?.Weight ?? item.ComboNavigation?.Weight) ?? 0) * item.Quantity),
                        TotalPrice = group.Sum(item =>
                        ((item.IosDeviceNavigation?.Price ?? item.ComboNavigation?.Price) ?? 0) * item.Quantity
                    ),
                    Items = group.ToList()
                    })
                    .ToList();

                var token = _configuration["GHTK:Token"];
                var baseUrl = "https://services-staging.ghtklab.com/services/shipment/fee";

                List<ShippingFeeResponse> shippingFees = new List<ShippingFeeResponse>();

                foreach (var shopGroup in groupedShops)
                {
                    var shopOwner = shopGroup.ShopOwner;
                    var totalWeight = shopGroup.TotalWeight;
                    var totalPrice = shopGroup.TotalPrice;
                    var shopAddress = await _unitOfWork.StoreRepository
                        .GetQueryable()
                        .Where(s => s.Id == shopOwner)
                        .Select(s => new { s.Address, s.ProvinceId, s.DistrictId, s.AddressId, s.WardId, s.Name })
                        .FirstOrDefaultAsync();

                    if (shopAddress == null) continue;

                    var provincesStore = await SyncProvincesAsync();
                    var provinceStore = provincesStore.FirstOrDefault(p => p.Id == shopAddress.ProvinceId);
                    var ProvinceNameStore = provinceStore?.Name ?? "Not found";

                    var districtsStore = await SyncDistrictsAsync(shopAddress.ProvinceId);
                    var districtStore = districtsStore.FirstOrDefault(d => d.Id == shopAddress.DistrictId);
                    var districtNameStore = districtStore?.Name ?? "Not found";

                    var wardsStore = await SyncWardsAsync(shopAddress.DistrictId);
                    var wardStore = wardsStore.FirstOrDefault(w => w.Id == shopAddress.WardId);
                    var wardnameStore = wardStore?.Name ?? "Not found";

                    var addressesStore = await SyncAddressAsync(shopAddress.WardId);
                    var addressStore = addressesStore.FirstOrDefault(w => w.Id == shopAddress.AddressId);
                    var addressNameStore = addressStore?.Name ?? "Not found";

                    var queryParams = $"?address={Uri.EscapeDataString(requestModel.Address)}" +
                                      $"&province={Uri.EscapeDataString(ProvinceNameCustomer)}" +
                                      $"&district={Uri.EscapeDataString(DistrictNameCustomer)}" +
                                      $"&pick_province={Uri.EscapeDataString(ProvinceNameStore)}" +
                                      $"&pick_district={Uri.EscapeDataString(districtNameStore)}" +
                                      $"&weight={totalWeight}" +
                                      $"&value={totalPrice}" +
                                      $"&deliver_option={requestModel.deliver_option}";

                    var requestUrl = baseUrl + queryParams;

                    var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    request.Headers.Add("Token", token);

                    var response = await _httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        shippingFees.Add(new ShippingFeeResponse { ShopOwnerId = shopOwner, Message = "Unable to get shipping fee" });
                        continue;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var feeData = JsonConvert.DeserializeObject<GHTKResponse>(responseContent);

                    if (feeData?.Fee == null)
                    {
                        shippingFees.Add(new ShippingFeeResponse { ShopOwnerId = shopOwner, Message = "No shipping cost data available" });
                        continue;
                    }

                    shippingFees.Add(new ShippingFeeResponse
                    {
                        ShopOwnerId = shopOwner,
                        Fee = feeData.Fee.Fee,
                        InsuranceFee = feeData.Fee.InsuranceFee,
                        ShipFeeOnly = feeData.Fee.ShipFeeOnly,
                        Message = $"Shipping Fee from Store {shopAddress.Name}"
                    });
                }
                var totalFee = shippingFees.Sum(fee => fee.Fee);

                shippingFees.Add(new ShippingFeeResponse
                {
                    ShopOwnerId = 99,
                    Fee = totalFee,
                    Message = "Total Shipping Fee"
                });

                return shippingFees;
            }
            catch (Exception ex)
            {
                return new List<ShippingFeeResponse> { new ShippingFeeResponse { Message = "System error" } };
            }
        } 


            /*public async Task<bool> CreateShipmentAsync(int orderId)
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
            }*/

            public async Task<bool> CreateShipmentAsync(int orderId)
        {
            try
            {
                var token = _configuration["GHTK:Token"];
                var baseUrl = "https://services-staging.ghtklab.com";
                var url = $"{baseUrl}/services/shipment/order/";

                var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

                var provinceCusId = order.ProvinceId;
                var districtCusId = order.DistrictId;
                var wardCusId = order.WardId;

                var provinceNameCustomer = await GetProvinceNameAsync(provinceCusId);
                var districtNameCustomer = await GetDistrictNameAsync(districtCusId, provinceCusId);
                var wardNameCustomer = await GetWardNameAsync(wardCusId,districtCusId);


                if (order == null)
                    return false;

                var loginUser = _userServices.GetLoginUser();
                if (loginUser == null || !await _userServices.CheckLoginUserRole(RoleEnum.STORE))
                {
                    throw new UnauthorizedAccessException("You don't have permission to create shipment for this order.");
                }

                var storeId = loginUser.Id;

                // Retrieve store information from database (assuming it's in the Store table)
                var store = await _unitOfWork.StoreRepository.GetByIdAsync(storeId);
                if (store == null)
                    throw new Exception("Store not found.");

                // Fetch province, district, and ward names using their IDs from the store
                var provinceId = store.ProvinceId;
                var districtId = store.DistrictId;
                var wardId = store.WardId;

                var provinceNameStore = await GetProvinceNameAsync(provinceId);
                var districtNameStore = await GetDistrictNameAsync(districtId, provinceId);
                var wardNameStore = await GetWardNameAsync(wardId, districtId);

                // Chỉ lấy OrderItems thuộc store của người dùng
                var storeOrderItems = order.OrderItems.Where(item => item.SellerId == storeId).ToList();
                if (storeOrderItems.Count == 0)
                    return false;

                var requestData = new
                {
                    products = storeOrderItems.Select(item => new
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
                        order_id = $"{orderId}",
                        pick_name = "Don Noi Thanh",
                        pick_address = order.Address,
                        pick_province = $"{provinceNameStore}",
                        pick_district = $"{districtNameStore}",
                        pick_ward = $"{wardNameStore}",
                        pick_tel = $"{store.ContactNumber}",
                        name = $"{order.ApplicationSerialNumber}",
                        address = $"{store.Address}",
                        province = $"{provinceNameCustomer}",
                        district = $"{districtNameCustomer}",
                        ward = $"{wardNameCustomer}",
                        tel = $"{order.ContactNumber}",
                        hamlet = "Khác",
                        is_freeship = 0,
                        pick_money = 0,
                        note = $"{order.Notes}",
                        value = (int)order.TotalPrice,
                    }
                };

                // Serialize and send request to GHTK API
                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestData, Newtonsoft.Json.Formatting.Indented);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

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


        public async Task<string> GetTrackingOrderAsync(int orderId)
        {
            try
            {
                var token = _configuration["GHTK:Token"];
                string requestUrl = $"https://services-staging.ghtklab.com/services/shipment/v2/{orderId}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Token", token);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                // Check if the response is successful
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    throw new Exception($"Không thể lấy mã vận đơn: {response.StatusCode}");
                }

                // Parse the response data
                var responseData = await response.Content.ReadAsStringAsync();
                var trackingData = JsonConvert.DeserializeObject<dynamic>(responseData);

                // Return the label_id if it exists
                return trackingData?.order?.label_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tracking order {orderId}: {ex.Message}");
                return null;
            }
        }

        public Task<byte[]> PrintLabelAsync(string trackingOrder)
        {
            throw new NotImplementedException();
        }

        public async Task<List<District>> SyncDistrictsAsync(int provinceId)
        {
            try
            {
                var token = _configuration["GHTK:Token"];
                var requestUrl = $"https://services-staging.ghtklab.com/services/address/getDeliveredAddress?parent_id={provinceId}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Token", token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    return new List<District>();
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<GHTKDistrictResponse>(responseContent);
                var districts = responseData?.data ?? new List<ApiDistrict>();

                return districts.Select(d => new District
                {
                    Id = d.id,
                    Name = d.name,
                    ProvinceId = provinceId
                }).ToList();
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
                var token = _configuration["GHTK:Token"]; // Lấy token từ cấu hình
                string requestUrl = "https://services-staging.ghtklab.com/services/address/getDeliveredAddress";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Token", token); // Thêm token vào header

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    return new List<Province>();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonResponse); // In ra JSON để kiểm tra

                var responseData = JsonConvert.DeserializeObject<LocationDataDTO>(jsonResponse);
                if (responseData == null || responseData.Data == null)
                {
                    Console.WriteLine("Error: API response is null or data is missing.");
                    return new List<Province>();
                }
                var provinces = responseData?.Data?.Select(apiProvince => new Province
                {
                    Id = apiProvince.Id,
                    Name = apiProvince.Name
                }).ToList() ?? new List<Province>();

                return provinces;
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
                var token = _configuration["GHTK:Token"];
                var requestUrl = $"https://services-staging.ghtklab.com/services/address/getDeliveredAddress?parent_id={districtId}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Token", token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    return new List<Ward>();
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<GHTKWardResponse>(responseContent);
                var wards = responseData?.data ?? new List<ApiWard>();

                return wards.Select(d => new Ward
                {
                    Id = d.Id,
                    Name = d.name,
                    ProvinceId = d.parent_id
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing wards for district {districtId}: {ex.Message}");
                return new List<Ward>();
            }
        }

        public async Task<List<Address>> SyncAddressAsync(int wardId)
        {
            try
            {
                var token = _configuration["GHTK:Token"];
                var requestUrl = $"https://services-staging.ghtklab.com/services/address/getDeliveredAddress?parent_id={wardId}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Token", token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
                    return new List<Address>();
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<GHTKAddressResponse>(responseContent);
                var addresses = responseData?.data ?? new List<ApiAddress>();

                return addresses.Select(d => new Address
                {
                    Id = d.Id,
                    Name = d.name,
                    WardId = d.parent_id
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing address for wards {wardId}: {ex.Message}");
                return new List<Address>();
            }
        }

        public async Task<string> GetProvinceNameAsync(int provinceId)
        {
            try
            {
                // Lấy danh sách tỉnh từ API
                var provinces = await SyncProvincesAsync();

                // Tìm tỉnh tương ứng với ProvinceId
                var province = provinces.FirstOrDefault(p => p.Id == provinceId);

                // Nếu tìm thấy, trả về tên tỉnh, nếu không trả về "Not found"
                return province?.Name ?? "Not found";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching province name: {ex.Message}");
                return "Not found";
            }
        }

        public async Task<string> GetDistrictNameAsync(int districtId, int provinceId)
        {
            try
            {
                // Lấy danh sách quận huyện từ API
                var districts = await SyncDistrictsAsync(provinceId); // Giả sử bạn có hàm SyncDistrictsAsync()

                // Tìm quận huyện tương ứng với districtId
                var district = districts.FirstOrDefault(d => d.Id == districtId);

                // Nếu tìm thấy, trả về tên quận huyện, nếu không trả về "Not found"
                return district?.Name ?? "Not found";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching district name: {ex.Message}");
                return "Not found";
            }
        }

        public async Task<string> GetWardNameAsync(int wardId,int districtId)
        {
            try
            {
                // Lấy danh sách phường từ API
                var wards = await SyncWardsAsync(districtId);

                // Tìm phường tương ứng với WardId
                var ward = wards.FirstOrDefault(w => w.Id == wardId);

                // Nếu tìm thấy, trả về tên phường, nếu không trả về "Not found"
                return ward?.Name ?? "Not found";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ward name: {ex.Message}");
                return "Not found";
            }
        }

    }
}
