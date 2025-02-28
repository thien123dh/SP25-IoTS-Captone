using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices userServices;
        private readonly IVNPayService vnpayServices;

        public OrderService(IUserServices userServices, IVNPayService vnpayServices)
        {
            _unitOfWork ??= new UnitOfWork();
            this.userServices = userServices;
            this.vnpayServices = vnpayServices;
        }

        private string GetApplicationSerialNumberOrder(int userID)
        {
            string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            return "OD{UserId}{CurrentDateTime}"
                .Replace("{UserId}", userID.ToString())
                .Replace("{CurrentDateTime}", currentDateTime);
        }


        public async Task<GenericResponseDTO<OrderResponeDTO>> CheckOrderSuccessfull(int? id, VNPayRequestDTO dto)
        {
            var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                return ResponseService<OrderResponeDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var selectedItems = await _unitOfWork.CartRepository
                .GetQueryable((int)loginUserId)
                .Where(item => item.CreatedBy == loginUserId && item.IsSelected)
                .Include(item => item.IosDeviceNavigation)
                .Include (item => item.ComboNavigation)
                .ToListAsync();

            string vnp_HashSecret = "UWEORVE5ULXN8YNCLM16TFK1FWPQ0SA9";

            var vnpayData = dto.urlResponse.Split("?")[1];
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (string s in vnpayData.Split("&"))
            {
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    var parts = s.Split("=");
                    if (parts.Length >= 2)
                    {
                        vnpay.AddResponseData(parts[0], parts[1]);
                    }
                }
            }


            string vnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");

            string decodedOrderInfo = Uri.UnescapeDataString(vnpay.GetResponseData("vnp_OrderInfo"));
            string[] orderDetails = decodedOrderInfo.Split('|');

            string address = orderDetails.Length > 0 ? orderDetails[0] : "";
            string contactPhone = orderDetails.Length > 1 ? orderDetails[1] : "";
            string notes = orderDetails.Length > 2 ? orderDetails[2] : "";

            string vnp_SecureHash = vnpay.GetResponseData("vnp_SecureHash");
            string terminalID = vnpay.GetResponseData("vnp_TmnCode");
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            string bankCode = vnpay.GetResponseData("vnp_BankCode");
            string transactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string txnRef = vnpay.GetResponseData("vnp_TxnRef");
            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string bankTranNo = vnpay.GetResponseData("vnp_BankTranNo");
            string cardType = vnpay.GetResponseData("vnp_CardType");
            string payDate = vnpay.GetResponseData("vnp_PayDate");
            string hashSecret = vnpay.GetResponseData("vnp_HashSecret");

            var responseCodeMessage = ReturnedErrorMessageResponseCode(responseCode);
            var transactionStatusMessage = ReturnedErrorMessageTransactionStatus(transactionStatus);


            VnPayResponse response = new VnPayResponse()
            {
                TransactionId = vnpayTranId,
                OrderInfo = decodedOrderInfo,
                Amount = vnp_Amount,
                BankCode = bankCode,
                BankTranNo = bankTranNo,
                CardType = cardType,
                PayDate = payDate,
                ResponseCode = responseCode,
                TransactionStatus = transactionStatus,
                TxnRef = txnRef
            };
            if (vnp_ResponseCode == "00" && transactionStatus == "00")
            {
               
            }
            else
            {
                return new GenericResponseDTO<OrderResponeDTO>
                {
                    IsSuccess = true,
                    Message = "Thanh toán thất bại.",
                    Data = null
                };
            }
            ResponsePayment payment = new ResponsePayment()
            {
                ResponseCodeMessage = responseCodeMessage,
                TransactionStatusMessage = transactionStatusMessage,
                VnPayResponse = response
            };
            var createTransactionPayment = new Orders
            {
                ApplicationSerialNumber = GetApplicationSerialNumberOrder(loginUserId),
                OrderBy = loginUserId,
                TotalPrice = vnp_Amount,
                Address = address,
                ContactNumber = contactPhone,
                Notes = notes,
                CreateDate = DateTime.Now,
                CreatedBy = loginUserId,
                UpdatedBy = loginUserId,
                OrderStatusId = (int)OrderStatusEnum.PENDING
            };
            _unitOfWork.OrderRepository.Create(createTransactionPayment);

            decimal ItemPrice = 0;
            foreach (var item in selectedItems)
            {
                var orderDetail = new OrderItem
                {
                    OrderId = createTransactionPayment.Id,
                    SellerId = item.SellerId,
                    OrderBy = loginUserId,
                    Quantity = item.Quantity,
                };
                // Xác định loại sản phẩm
                if (item.IosDeviceNavigation != null)
                {
                    orderDetail.ProductType = (int)ProductTypeEnum.IOT_DEVICE;
                    orderDetail.IosDeviceId = item.IosDeviceNavigation.Id;
                    orderDetail.Price = item.IosDeviceNavigation?.Price ?? 0m;
                }
                else if (item.ComboNavigation != null)
                {
                    orderDetail.ProductType = (int)ProductTypeEnum.COMBO;
                    orderDetail.ComboId = item.ComboNavigation.Id;
                    orderDetail.Price = item.ComboNavigation?.Price ?? 0m;
                }
                else if (item.LabNavigation != null)
                {
                    orderDetail.ProductType = (int)ProductTypeEnum.LAB;
                    orderDetail.LabId = item.LabNavigation.Id;
                    orderDetail.Price = item.LabNavigation?.Price ?? 0m;
                }
                else
                {
                    throw new Exception("Cannot Add to Order this Product. Please try again");
                }

                _unitOfWork.OrderDetailRepository.Create(orderDetail);
            }
            /*await _unitOfWork.CartRepository.RemoveAsync(selectedItems);
            await _unitOfWork.CartRepository.SaveAsync();*/

            return new GenericResponseDTO<OrderResponeDTO>
            {
                IsSuccess = true,
                Message = "Đơn hàng đã được xác nhận thành công và sản phẩm đã được xoá khỏi giỏ hàng.",
                Data = null
            };

        }

        public async Task<GenericResponseDTO<OrderResponeDTO>> CreateOrder(int? id, OrderRequestDTO payload, string returnUrl)
        {
            try
            {
                var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                return ResponseService<OrderResponeDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var address = payload.Address;
            var contactPhone = payload.ContactNumber;
            var notes = payload.Notes;

            // Lấy danh sách sản phẩm được chọn trong giỏ hàng
            var selectedItems = await _unitOfWork.CartRepository
                .GetQueryable((int)loginUserId)
                .Where(item => item.CreatedBy == loginUserId && item.IsSelected)
                .Include(item => item.IosDeviceNavigation)
                .Include(item => item.ComboNavigation)
                .ToListAsync();

            if (selectedItems == null || !selectedItems.Any())
            {
                return new GenericResponseDTO<OrderResponeDTO>
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
                return new GenericResponseDTO<OrderResponeDTO>
                {
                    IsSuccess = false,
                    Message = "Giá trị đơn hàng không hợp lệ.",
                    Data = null
                };
            }
                var orderInfo = new
                {
                    Address = address,
                    ContactPhone = contactPhone,
                    Notes = notes
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
                return new GenericResponseDTO<OrderResponeDTO>
                {
                    IsSuccess = true,
                    Message = "Tạo đơn hàng thành công, vui lòng thanh toán.",
                    Data = new OrderResponeDTO { PaymentUrl = paymentUrl } 
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

        public Task<GenericResponseDTO<OrderResponeDTO>> getOrderDetailsByOrderId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseDTO<OrderResponeDTO>> GetOrdersByUserID(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
