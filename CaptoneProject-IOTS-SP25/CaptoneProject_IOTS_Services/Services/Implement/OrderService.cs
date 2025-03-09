using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices userServices;
        private readonly IVNPayService vnpayServices;
        private readonly IGHTKService _ghtkService;

        public OrderService(IUserServices userServices, IVNPayService vnpayServices, IGHTKService ghtkService)
        {
            _unitOfWork ??= new UnitOfWork();
            this.userServices = userServices;
            this.vnpayServices = vnpayServices;
            this._ghtkService = ghtkService;
        }

        private string GetApplicationSerialNumberOrder(int userID)
        {
            string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            return "OD{UserId}{CurrentDateTime}"
                .Replace("{UserId}", userID.ToString())
                .Replace("{CurrentDateTime}", currentDateTime);
        }


        public async Task<GenericResponseDTO<OrderReturnPaymentDTO>> CheckOrderSuccessfull(int? id, VNPayRequestDTO dto)
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                return ResponseService<OrderReturnPaymentDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;
            var selectedItems = await _unitOfWork.CartRepository
                .GetQueryable((int)loginUserId)
                .Where(item => item.CreatedBy == loginUserId && item.IsSelected)
                .Include(item => item.IosDeviceNavigation)
                .Include(item => item.ComboNavigation)
                .ToListAsync();

            string vnpayData = dto.urlResponse.Split("?")[1];
            VnPayLibrary vnpay = new VnPayLibrary();
            foreach (string s in vnpayData.Split("&"))
            {
                var parts = s.Split("=");
                if (parts.Length >= 2 && parts[0].StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(parts[0], parts[1]);
                }
            }

            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string encodedOrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            string address = "", contactPhone = "", notes = "";
            int provinceId = 0, districtId = 0, wardId = 0;
            string provinceName = "", districtName = "", wardName = "", fullAddress = "";

            if (!string.IsNullOrEmpty(encodedOrderInfo))
            {
                try
                {
                    string decodedUrl = HttpUtility.UrlDecode(encodedOrderInfo);
                    byte[] data = Convert.FromBase64String(decodedUrl);
                    string jsonString = Encoding.UTF8.GetString(data);
                    var orderInfo = JsonConvert.DeserializeObject<OrderInfo>(jsonString);

                    address = orderInfo?.Address ?? "";
                    contactPhone = orderInfo?.ContactNumber ?? "";
                    notes = orderInfo?.Notes ?? "";
                    provinceId = orderInfo?.ProvinceId ?? 0;
                    provinceName = orderInfo?.ProvinceName ?? "";
                    districtId = orderInfo?.DistrictId ?? 0;
                    districtName = orderInfo?.DistrictName ?? "";
                    wardId = orderInfo?.WardId ?? 0;
                    wardName = orderInfo?.WardName ?? "";
                    fullAddress = orderInfo?.FullAddress ?? "";
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid Base64 format for vnp_OrderInfo. Using raw value.");
                }
            }

            if (vnp_ResponseCode != "00" || vnp_TransactionStatus != "00")
            {
                return new GenericResponseDTO<OrderReturnPaymentDTO>
                {
                    IsSuccess = false,
                    Message = "Thanh toán thất bại.",
                    Data = null
                };
            }

            var createTransactionPayment = new Orders
            {
                ApplicationSerialNumber = GetApplicationSerialNumberOrder(loginUserId),
                OrderBy = loginUserId,
                TotalPrice = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100,
                Address = address,
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardId = wardId,
                ContactNumber = contactPhone,
                Notes = notes,
                CreateDate = DateTime.Now,
                CreatedBy = loginUserId,
                UpdatedBy = loginUserId,
                OrderStatusId = (int)OrderStatusEnum.SUCCESS_TO_ORDER
            };
            _unitOfWork.OrderRepository.Create(createTransactionPayment);

            foreach (var item in selectedItems)
            {
                var orderDetail = new OrderItem
                {
                    OrderId = createTransactionPayment.Id,
                    SellerId = item.SellerId,
                    OrderBy = loginUserId,
                    Quantity = item.Quantity,
                    ProductType = item.IosDeviceNavigation != null ? (int)ProductTypeEnum.IOT_DEVICE :
                                  item.ComboNavigation != null ? (int)ProductTypeEnum.COMBO :
                                  item.LabNavigation != null ? (int)ProductTypeEnum.LAB : throw new Exception("Invalid product type"),
                    IosDeviceId = item.IosDeviceNavigation?.Id,
                    ComboId = item.ComboNavigation?.Id,
                    LabId = item.LabNavigation?.Id,
                    Price = item.IosDeviceNavigation?.Price ?? item.ComboNavigation?.Price ?? item.LabNavigation?.Price ?? 0m,
                    OrderItemStatus = (int)OrderItemStatusEnum.PENDING
                };
                _unitOfWork.OrderDetailRepository.Create(orderDetail);
            }

            var orderReturnPaymentDTO = new OrderReturnPaymentDTO
            {
                ApplicationSerialNumber = createTransactionPayment.ApplicationSerialNumber,
                TotalPrice = createTransactionPayment.TotalPrice,
                ProvinceId = createTransactionPayment.ProvinceId,
                ProvinceName = provinceName,
                DistrictId = createTransactionPayment.DistrictId,
                DistrictName = districtName,
                WardId = createTransactionPayment.WardId,
                WardName = wardName,
                Address = createTransactionPayment.Address,
                ContactNumber = createTransactionPayment.ContactNumber,
                Notes = createTransactionPayment.Notes,
                CreateDate = createTransactionPayment.CreateDate,
                OrderStatusId = createTransactionPayment.OrderStatusId
            };
/*            await _unitOfWork.CartRepository.RemoveAsync(selectedItems);
            await _unitOfWork.CartRepository.SaveAsync();*/
            return new GenericResponseDTO<OrderReturnPaymentDTO>
            {
                IsSuccess = true,
                Message = "Đơn hàng đã được thanh toán thành công.",
                Data = orderReturnPaymentDTO
            };
        }


        public async Task<GenericResponseDTO<OrderReturnPaymentDTO>> CreateOrder(int? id, OrderRequestDTO payload, string returnUrl)
        {
            try
            {
                var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                return ResponseService<OrderReturnPaymentDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var address = payload.Address;
            var contactPhone = payload.ContactNumber;
            var notes = payload.Notes;

                var provinces = await _ghtkService.SyncProvincesAsync();
                var province = provinces.FirstOrDefault(p => p.Id == payload.ProvinceId);
                if (province == null)
                    return ResponseService<OrderReturnPaymentDTO>.BadRequest("Invalid Province ID");

                var districts = await _ghtkService.SyncDistrictsAsync(payload.ProvinceId);
                var district = districts.FirstOrDefault(d => d.Id == payload.DistrictId);
                if (district == null)
                    return ResponseService<OrderReturnPaymentDTO>.BadRequest("Invalid District ID");

                var wards = await _ghtkService.SyncWardsAsync(payload.DistrictId);
                var ward = wards.FirstOrDefault(w => w.Id == payload.WardId);
                if (ward == null)
                    return ResponseService<OrderReturnPaymentDTO>.BadRequest("Invalid Ward ID");

                // Lấy danh sách sản phẩm được chọn trong giỏ hàng
                var selectedItems = await _unitOfWork.CartRepository
                .GetQueryable((int)loginUserId)
                .Where(item => item.CreatedBy == loginUserId && item.IsSelected)
                .Include(item => item.IosDeviceNavigation)
                .Include(item => item.ComboNavigation)
                .ToListAsync();

            if (selectedItems == null || !selectedItems.Any())
            {
                return new GenericResponseDTO<OrderReturnPaymentDTO>
                {
                    IsSuccess = false,
                    Message = "Giỏ hàng trống hoặc không có sản phẩm nào được chọn.",
                    Data = null
                };
            }

            var totalPrice = selectedItems.Sum(item =>
                (((item.IosDeviceNavigation?.Price ?? 0m) * item.Quantity) + ((item.ComboNavigation?.Price ?? 0m) * item.Quantity))
            );

            if (totalPrice <= 0)
            {
                return new GenericResponseDTO<OrderReturnPaymentDTO>
                {
                    IsSuccess = false,
                    Message = "Giá trị đơn hàng không hợp lệ.",
                    Data = null
                };
            }
                var orderInfo = new OrderInfo
                {
                    Address = address,
                    ContactNumber = contactPhone,
                    Notes = notes,
                    ProvinceId = province.Id,
                    ProvinceName = province.Name,

                    DistrictId = district.Id,
                    DistrictName = district.Name,

                    WardId = ward.Id,
                    WardName = ward.Name,

                    FullAddress = $"{payload.Address}, {ward.Name}, {district.Name}, {province.Name}"
                };


                string vnp_ReturnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "https://localhost:44346/checkout-process-order";
                string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string vnp_TmnCode = "2BF25S7Y";
                string vnp_HashSecret = "UWEORVE5ULXN8YNCLM16TFK1FWPQ0SA9";

                string orderInfoJson = JsonConvert.SerializeObject(orderInfo);
                string encryptedOrderInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(orderInfoJson));

                if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                {
                    throw new Exception("Merchant code or secret key is missing.");
                }

                // Fix cứng số tiền theo loại thanh toán
                var amounts = ((long)totalPrice * 100).ToString();
                var vnp_TxnRef = $"{loginUserId}{DateTime.Now:HHmmss}";
                var vnp_Amount = amounts;

                var vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", vnp_Amount);
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                vnpay.AddRequestData("vnp_CreateDate", vietnamTime.AddMinutes(-20).ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", encryptedOrderInfo);
                vnpay.AddRequestData("vnp_OrderType", "order");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
                vnpay.AddRequestData("vnp_TxnRef", vnp_TxnRef);
                vnpay.AddRequestData("vnp_ExpireDate", vietnamTime.AddMinutes(5).ToString("yyyyMMddHHmmss"));
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                return new GenericResponseDTO<OrderReturnPaymentDTO>
                {
                    IsSuccess = true,
                    Message = "Tạo đơn hàng thành công, vui lòng thanh toán.",
                    Data = new OrderReturnPaymentDTO { PaymentUrl = paymentUrl } 
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during payment: {ex.Message}");
            }
        }

        private string ReturnedErrorMessageResponseCode(string code)
        {
            switch (code)
            {
                case "00": return "Giao dịch thành công";
                case "07": return "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).";
                case "09": return "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.";
                case "10": return "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần.";
                case "11": return "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.";
                case "12": return "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.";
                case "13": return "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.";
                case "24": return "Giao dịch không thành công do: Khách hàng hủy giao dịch.";
                case "51": return "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.";
                case "65": return "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.";
                case "75": return "Ngân hàng thanh toán đang bảo trì.";
                case "79": return "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch.";
                case "99": return "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê).";
                default: return "Mã lỗi không hợp lệ";
            }
        }

        private string ReturnedErrorMessageTransactionStatus(string code)
        {
            switch (code)
            {
                case "00": return "Giao dịch thành công";
                case "01": return "Giao dịch chưa hoàn tất";
                case "02": return "Giao dịch bị lỗi";
                case "04": return "Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)";
                case "05": return "VNPAY đang xử lý giao dịch này (GD hoàn tiền)";
                case "06": return "VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)";
                case "07": return "Giao dịch bị nghi ngờ gian lận";
                case "09": return "GD Hoàn trả bị từ chối";
                default: return "Mã lỗi không hợp lệ";
            }
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetOrdersByUserPagination(int? filterOrderId, PaginationRequest payload)
        {
            try
            {
                var loginUser = userServices.GetLoginUser();

                if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                    return ResponseService<PaginationResponseDTO<OrderResponseDTO>>.Unauthorize("You don't have permission to access");

                var loginUserId = loginUser.Id;

                var orders = await _unitOfWork.OrderRepository.GetByUserIdAsync(loginUserId);

                if (orders == null || !orders.Any())
                    return ResponseService<PaginationResponseDTO<OrderResponseDTO>>.NotFound("No orders found for this user.");

                var filteredOrders = orders.Where(order =>
                    (filterOrderId == null || order.Id == filterOrderId)
                );


                int totalOrders = filteredOrders.Count();

                var paginatedOrders = filteredOrders
                    .OrderByDescending(order => order.CreateDate)
                    .Skip((payload.PageIndex - 1) * payload.PageSize)
                    .Take(payload.PageSize)
                    .ToList();

                // Chuyển đổi danh sách Order thành OrderResponseDTO
                var orderDTOs = paginatedOrders.Select(order => new OrderResponseDTO
                {
                    ApplicationSerialNumber = order.ApplicationSerialNumber,
                    TotalPrice = order.TotalPrice,
                    Address = order.Address,
                    ContactNumber = order.ContactNumber,
                    Notes = order.Notes,
                    CreateDate = order.CreateDate,
                    UpdatedDate = order.UpdatedDate,
                    OrderStatusId = order.OrderStatusId
                }).ToList();

                var paginationResponse = new PaginationResponseDTO<OrderResponseDTO>
                {
                    Data = orderDTOs,
                    TotalCount = totalOrders,
                    PageIndex = payload.PageIndex,
                    PageSize = payload.PageSize
                };

                return ResponseService<PaginationResponseDTO<OrderResponseDTO>>.OK(paginationResponse);
            }
            catch (Exception ex)
            {
                return ResponseService<PaginationResponseDTO<OrderResponseDTO>>.BadRequest("Cannot get orders. Please try again.");
            }
        }
    }
}
