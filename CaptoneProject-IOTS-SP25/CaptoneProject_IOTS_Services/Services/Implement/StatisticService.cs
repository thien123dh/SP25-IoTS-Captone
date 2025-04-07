using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Request;
using CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Response;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class StatisticService : IStatisticService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;

        public StatisticService(UnitOfWork unitOfWork, IUserServices userServices)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
        }

        public async Task<StatisticDto> GetStatisticBy(
            StatisticRequest request,
            Expression<Func<User, bool>> userFilter,
            Expression<Func<IotsDevice, bool>> deviceFilter,
            Expression<Func<Combo, bool>> comboFilter,
            Expression<Func<Lab, bool>> labFilter,
            Expression<Func<Orders, bool>> orderFilter)
        {
            var userQuery = unitOfWork.UserRepository.Search(userFilter)
                            .Include(u => u.UserRoles)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate) && item.IsActive == 1);
            var totalActiveUsers = userQuery.Count();

            var totalStores = userQuery.Where(u => u.UserRoles != null && u.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.STORE)).Count();
            var totalTrainers = userQuery.Where(u => u.UserRoles != null && u.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.TRAINER)).Count();
            var totalCustomers = userQuery.Where(u => u.UserRoles != null && u.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.CUSTOMER)).Count();

            var totalDevices = unitOfWork.IotsDeviceRepository.Search(deviceFilter)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate) && item.IsActive == 1)
                            .Count();

            var totalCombos = unitOfWork.ComboRepository.Search(comboFilter)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate) && item.IsActive == 1)
                            .Count();

            var labQuery = unitOfWork.LabRepository.Search(labFilter)
                            .Where(item => (request.StartDate <= item.CreatedDate && item.CreatedDate <= request.EndDate));

            var totalLabs = labQuery.Count();
            var totalPendingToApproveLabs = labQuery.Where(item => item.Status == (int)LabStatusEnum.PENDING_TO_APPROVE).Count();
            var totalActiveLabs = labQuery.Where(item => item.Status == (int)LabStatusEnum.APPROVED).Count();
            var totalRejectedLabs = labQuery.Where(item => item.Status == (int)LabStatusEnum.REJECTED).Count();

            var orderQuery = unitOfWork.OrderRepository.Search(orderFilter)
                            .Include(item => item.OrderItems)
                            .Where(item => (request.StartDate <= item.CreateDate && item.CreateDate <= request.EndDate));

            var totalOrders  = orderQuery.Count();
            var totalCashpaymentOrders = orderQuery.Where(item => item.OrderStatusId == (int)OrderStatusEnum.CASH_PAYMENT).Count();
            var totalVnpayOrders = orderQuery.Where(item => item.OrderStatusId == (int)OrderStatusEnum.SUCCESS_TO_ORDER).Count();
            var totalCancelledOrders = orderQuery.Where(item => item.OrderStatusId == (int)OrderStatusEnum.CANCELLED).Count();
            var totalSuccessOrders = orderQuery
                .Where(item => !item.OrderItems.Any(oi => oi.OrderItemStatus != (int)OrderItemStatusEnum.SUCCESS_ORDER))
                .Count();
            var totalBadFeedbackOrders = orderQuery
                .Where(item => item.OrderItems.Any(oi => oi.OrderItemStatus == (int)OrderItemStatusEnum.BAD_FEEDBACK))
                .Count();

            return new StatisticDto
            {
                TotalActiveUsers = totalActiveUsers,
                TotalCustomerUsers = totalCustomers,
                TotalStoreUsers = totalStores,
                TotalTrainerUsers = totalTrainers,
                TotalActiveCombos = totalCombos,
                TotalActiveDevices = totalDevices,
                TotalLabs = totalLabs,
                TotalActiveLabs = totalLabs,
                TotalPendingToApproveLabs = totalPendingToApproveLabs,
                TotalRejectedLabs = totalRejectedLabs,
                TotalCashpaymentOrders = totalCashpaymentOrders,
                TotalSuccessOrders = totalSuccessOrders,
                TotalOrders = totalOrders,
                TotalCancelledOrders = totalCancelledOrders,
                TotalVnPayOrders = totalVnpayOrders,
                TotalIncludedBadFeedbackOrders = totalBadFeedbackOrders
            };
        }

        public async Task<ResponseDTO> GetAdminStatistic(StatisticRequest request)
        {
            var result = await GetStatisticBy(
                request,
                user => true,
                device => true,
                combo => true,
                lab => true,
                order => true
            ).ConfigureAwait(false);

            var sumOrderItems = (unitOfWork.OrderDetailRepository.Search(
                item => (request.StartDate <= item.Order.CreateDate && item.Order.CreateDate <= request.EndDate) &&
                            item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER)
                .Include(item => item.Order)
                .Sum(item => item.Price / 1000 * item.Quantity));

            var totalOrderRevenue = (sumOrderItems * ((decimal)ApplicationConst.FEE_PER_PRODUCT / 100));

            var packageRevenue = unitOfWork.AccountMembershipPackageRepository.Search(
                item => request.StartDate <= item.LastPaymentDate && item.LastPaymentDate <= request.EndDate
            ).Sum(item => item.Fee);

            var totalRevenue = totalOrderRevenue + packageRevenue;

            result.TotalRevenue = totalRevenue;

            return ResponseService<object>.OK(result);
        }

        public async Task<ResponseDTO> GetStoreStatistic(StatisticRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();

            var result = await GetStatisticBy(
                request,
                user => false,
                device => device.CreatedBy == loginUserId,
                combo => combo.CreatedBy == loginUserId,
                lab => false,
                order => order.OrderItems.Any(oi => oi.SellerId == loginUserId)
            ).ConfigureAwait(false);

            var sumOrderItems = (unitOfWork.OrderDetailRepository.Search(
                item => (request.StartDate <= item.Order.CreateDate && item.Order.CreateDate <= request.EndDate) &&
                            item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER && 
                            item.SellerId == loginUserId)
                .Include(item => item.Order)
                .Sum(item => item.Price / 1000 * item.Quantity));

            var revenue = sumOrderItems - (sumOrderItems * ((decimal)ApplicationConst.FEE_PER_PRODUCT / 100));

            result.TotalRevenue = revenue;

            return ResponseService<object>.OK(result);
        }

        public async Task<ResponseDTO> GetTrainerStatistic(StatisticRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();
           
            var result = await GetStatisticBy(
                request,
                user => false,
                device => false,
                combo => false,
                lab => lab.CreatedBy == loginUserId,
                order => order.OrderItems.Any(oi => oi.SellerId == loginUserId)
            ).ConfigureAwait(false);

            var sumOrderItems = (unitOfWork.OrderDetailRepository.Search(
                item => (request.StartDate <= item.Order.CreateDate && item.Order.CreateDate <= request.EndDate) &&
                            item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER &&
                            item.SellerId == loginUserId)
                .Include(item => item.Order)
                .Sum(item => item.Price / 1000 * item.Quantity));

            var revenue = sumOrderItems * (sumOrderItems * ((decimal)ApplicationConst.FEE_PER_PRODUCT / 100));

            result.TotalRevenue = revenue;

            return ResponseService<object>.OK(result);
        }
    }
}
