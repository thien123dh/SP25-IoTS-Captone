using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Request;
using CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Response;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class StatisticService : IStatisticService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly int APPLICATION_FEE;

        public StatisticService(UnitOfWork unitOfWork, IUserServices userServices)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;

            APPLICATION_FEE = this.unitOfWork.GeneralSettingsRepository.Search(item => true)?.FirstOrDefault()?.ApplicationFeePercent ?? 0;
        }

        public async Task<StatisticDto> GetStatisticBy(
            StatisticRequestRange request,
            Expression<Func<User, bool>> userFilter,
            Expression<Func<IotsDevice, bool>> deviceFilter,
            Expression<Func<Combo, bool>> comboFilter,
            Expression<Func<Lab, bool>> labFilter,
            Expression<Func<Orders, bool>> orderFilter,
            Expression<Func<Feedback, bool>> feedbackFilter)
        {
            var userQuery = unitOfWork.UserRepository.Search(userFilter)
                            .Include(u => u.UserRoles)
                            .Where(item => item.IsActive == 1);
            var totalActiveUsers = userQuery.Count();

            var totalStores = userQuery.Where(u => u.UserRoles != null && u.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.STORE)).Count();
            var totalTrainers = userQuery.Where(u => u.UserRoles != null && u.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.TRAINER)).Count();
            var totalCustomers = userQuery.Where(u => u.UserRoles != null && u.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.CUSTOMER)).Count();

            var deviceQuery = unitOfWork.IotsDeviceRepository.Search(deviceFilter)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate) && item.IsActive == 1);

            var comboQuery = unitOfWork.ComboRepository.Search(comboFilter)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate) && item.IsActive == 1);

            var labQuery = unitOfWork.LabRepository.Search(labFilter)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate));

            var totalActiveLabs = labQuery.Where(item => item.Status == (int)LabStatusEnum.APPROVED).Count();
            var totalRejectedLabs = labQuery.Where(item => item.Status == (int)LabStatusEnum.REJECTED).Count();

            var orderQuery = unitOfWork.OrderRepository.Search(orderFilter)
                            .Include(item => item.OrderItems)
                            .Where(item => (request.StartDate <= item.CreateDate && item.CreateDate <= request.EndDate));

            var feedbackQuery = unitOfWork.FeedbackRepository.Search(feedbackFilter)
                            .Include(item => item.OrderItem)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate));

            var totalOrders = orderQuery.Count();
            var totalCashpaymentOrders = orderQuery.Where(item => item.OrderStatusId == (int)OrderStatusEnum.CASH_PAYMENT).Count();
            var totalVnpayOrders = orderQuery.Where(item => item.OrderStatusId == (int)OrderStatusEnum.SUCCESS_TO_ORDER).Count();
            var totalCancelledOrders = orderQuery.Where(item => item.OrderStatusId == (int)OrderStatusEnum.CANCELLED).Count();
            var totalSuccessOrders = orderQuery
                .Where(item => !item.OrderItems.Any(oi => oi.OrderItemStatus != (int)OrderItemStatusEnum.SUCCESS_ORDER))
                .Count();
            var totalBadFeedbackOrders = orderQuery
                .Where(item => item.OrderItems.Any(oi => oi.OrderItemStatus == (int)OrderItemStatusEnum.BAD_FEEDBACK))
                .Count();

            var res = new StatisticDto
            {
                TotalActiveUsers = totalActiveUsers,
                TotalCustomerUsers = totalCustomers,
                TotalStoreUsers = totalStores,
                TotalTrainerUsers = totalTrainers,
                TotalCashpaymentOrders = totalCashpaymentOrders,
                TotalSuccessOrders = totalSuccessOrders,
                TotalOrders = totalOrders,
                TotalCancelledOrders = totalCancelledOrders,
                TotalVnPayOrders = totalVnpayOrders,
                TotalIncludedBadFeedbackOrders = totalBadFeedbackOrders,
                TotalActiveDevices = deviceQuery.Count(),
                TotalActiveCombos = comboQuery.Count(),
                TotalActiveLabs = labQuery.Count(),
                TotalFeedbacks = feedbackQuery.Count(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };

            foreach (var time in request.TimeList)
            {
                var devices = deviceQuery.Where(d => time.StartDate.Date <= ((DateTime)d.CreatedDate).Date && ((DateTime)d.CreatedDate).Date <= time.EndDate.Date).Count();
                var combos = comboQuery.Where(d => time.StartDate.Date <= ((DateTime)d.CreatedDate).Date && ((DateTime)d.CreatedDate).Date <= time.EndDate.Date).Count();
                var labs = labQuery.Where(d => time.StartDate.Date <= ((DateTime)d.CreatedDate).Date && ((DateTime)d.CreatedDate).Date <= time.EndDate.Date).Count();

                res.StatisticProducts.Add(new DtoStatisticProduct
                {
                    Devices = devices,
                    Combos = combos,
                    Labs = labs,
                    Time = time.Time,
                    StartDate = time.StartDate,
                    EndDate = time.EndDate
                });
            }

            return res;
        }

        public async Task<ResponseDTO> GetAdminStatistic(StatisticRequest request)
        {
            var requestRange = BuildToStatisticRequestRange(request);

            var result = await GetStatisticBy(
                requestRange,
                user => true,
                device => true,
                combo => true,
                lab => true,
                order => true,
                feedback => true
            ).ConfigureAwait(false);

            var sumOrderItems = (unitOfWork.OrderDetailRepository.Search(
                item => (requestRange.StartDate <= item.Order.CreateDate && item.Order.CreateDate <= requestRange.EndDate) &&
                            item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER)
                .Include(item => item.Order)
                .Sum(item => item.Price / 1000 * item.Quantity));

            //var totalOrderRevenue = (sumOrderItems * ((decimal)APPLICATION_FEE / 100));

            //var totalRevenue = unitOfWork.TransactionRepository.Search(tran => tran.IsApplication > 0 && tran.Amount > 0).Sum(t => t.Amount);

            //var packageRevenue = unitOfWork.AccountMembershipPackageRepository.Search(
            //    item => requestRange.StartDate <= item.LastPaymentDate && item.LastPaymentDate <= requestRange.EndDate
            //).Sum(item => item.Fee);

            //var totalRevenue = totalOrderRevenue + packageRevenue;
            
            var totalRevenue = unitOfWork.TransactionRepository.Search(item => item.IsApplication > 0 && item.Amount > 0
                && requestRange.StartDate <= item.CreatedDate && item.CreatedDate <= requestRange.EndDate
            ).Sum(item => item.Amount);

            result.TotalRevenue = totalRevenue;

            return ResponseService<object>.OK(result);
        }

        public async Task<ResponseDTO> GetStoreStatistic(StatisticRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();

            var requestRange = BuildToStatisticRequestRange(request);

            var result = await GetStatisticBy(
                requestRange,
                user => false,
                device => device.CreatedBy == loginUserId,
                combo => combo.CreatedBy == loginUserId,
                lab => false,
                order => order.OrderItems.Any(oi => oi.SellerId == loginUserId),
                feedback => feedback.OrderItem.SellerId == loginUserId
            ).ConfigureAwait(false);

            var sumOrderItems = (unitOfWork.OrderDetailRepository.Search(
                item => (requestRange.StartDate <= item.Order.CreateDate && item.Order.CreateDate <= requestRange.EndDate) &&
                            item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER &&
                            item.SellerId == loginUserId)
                .Include(item => item.Order)
                .Sum(item => item.Price / 1000 * item.Quantity));

            //var revenue = sumOrderItems * ((100 - APPLICATION_FEE) / 100);
            var revenue = unitOfWork.TransactionRepository.Search(item => item.UserId == loginUserId && item.Amount > 0
                && requestRange.StartDate <= item.CreatedDate && item.CreatedDate <= requestRange.EndDate
            ).Sum(item => item.Amount);

            result.TotalRevenue = revenue;

            return ResponseService<object>.OK(result);
        }

        public async Task<ResponseDTO> GetTrainerStatistic(StatisticRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();

            var requestRange = BuildToStatisticRequestRange(request);

            var result = await GetStatisticBy(
                requestRange,
                user => false,
                device => false,
                combo => false,
                lab => lab.CreatedBy == loginUserId,
                order => order.OrderItems.Any(oi => oi.SellerId == loginUserId),
                f => f.OrderItem.SellerId == loginUserId
            ).ConfigureAwait(false);

            var sumOrderItems = (unitOfWork.OrderDetailRepository.Search(
                item => (requestRange.StartDate <= item.Order.CreateDate && item.Order.CreateDate <= requestRange.EndDate) &&
                            item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER &&
                            item.SellerId == loginUserId)
                .Include(item => item.Order)
                .Sum(item => item.Price / 1000 * item.Quantity));

            //var revenue = sumOrderItems * ((decimal)(100 - APPLICATION_FEE) / 100);

            var revenue = unitOfWork.TransactionRepository.Search(item => item.UserId == loginUserId && item.Amount > 0
                && requestRange.StartDate <= item.CreatedDate && item.CreatedDate <= requestRange.EndDate
            ).Sum(item => item.Amount);
            
            result.TotalRevenue = revenue;

            return ResponseService<object>.OK(result);
        }

        public StatisticRequestRange BuildToStatisticRequestRange(StatisticRequest request)
        {
            var year = request.Year;
            var month = request.Month;

            var res = new StatisticRequestRange();

            if (request.Month == null)
            {
                int currentYear = request.Year;
                DateTime startDate = new DateTime(currentYear, 1, 1);
                DateTime endDate = new DateTime(currentYear, 12, 31);

                res = new StatisticRequestRange
                {
                    EndDate = endDate,
                    StartDate = startDate
                };

                //12 months
                for (int i = 1; i <= 12; ++i)
                {
                    res.TimeList.Add(BuildToDtoTimesStartEndDate(currentYear, i));
                }

                return res;
            }


            DateTime firstDay = new DateTime(year, (int)month, 1);
            DateTime lastDay = new DateTime(year, (int)month, DateTime.DaysInMonth(year, (int)month));

            res = new StatisticRequestRange
            {
                EndDate = lastDay,
                StartDate = firstDay,
            };

            int daysInMonth = DateTime.DaysInMonth(year, (int)month);
            DateTime currentDate = new DateTime(year, (int)month, 1);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var dateName = currentDate.ToString("yyyy-MM-dd");
                res.TimeList.Add(new DtoTimesStartEndDate
                {
                    StartDate = currentDate,
                    EndDate = currentDate,
                    Time = dateName,
                });
                currentDate = currentDate.AddDays(1);
            }

            return res;
        }

        public DtoTimesStartEndDate BuildToDtoTimesStartEndDate(int year, int month)
        {
            DateTime firstDayOfMonth = new DateTime(year, (int)month, 1);
            DateTime lastDayOfMonth = new DateTime(year, (int)month, DateTime.DaysInMonth(year, (int)month));
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)month);

            return new DtoTimesStartEndDate
            {
                StartDate = firstDayOfMonth,
                EndDate = lastDayOfMonth,
                Time = monthName
            };
        }
    }
}
