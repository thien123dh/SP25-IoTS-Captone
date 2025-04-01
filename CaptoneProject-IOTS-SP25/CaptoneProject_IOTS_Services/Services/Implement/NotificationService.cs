using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.NotificationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class NotificationService : INotificationService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly short MAX_RECORD = 50;

        public NotificationService(UnitOfWork unitOfWork, IUserServices userServices)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
        }

        public async Task<ResponseDTO> CountNotReadNotification()
        {
            var loginUserId = userServices.GetLoginUserId();

            var count = unitOfWork.NotificationRepository
                .Search(item => item.IsRead && item.ReceiverId == loginUserId)
                .Count();

            return ResponseService<object>.OK(count);
        }

        public Notifications BuildNotification(MapService<NotificationRequestDTO, Notifications> mapper, NotificationRequestDTO source, int receiverId)
        {
            var res = mapper.MappingTo(source);

            res.CreatedDate = DateTime.Now;
            res.IsRead = false;
            res.ReceiverId = receiverId;

            return res;
        }

        public async Task<ResponseDTO> CreateUserNotification(List<NotificationRequestDTO> request)
        {
            var mapper = new MapService<NotificationRequestDTO, Notifications>();

            var saveList = request.Select(
                item => BuildNotification(mapper, item, item.ReceiverId)
            );

            try
            {
                await unitOfWork.NotificationRepository.CreateAsync(saveList);
            }
            catch (Exception ex)
            {
                ResponseService<object>.BadRequest(ex.Message);
            }

            return ResponseService<object>.OK(null);
        }

        public async Task<ResponseDTO> GetNotifications()
        {
            var loginUserId = userServices.GetLoginUserId();

            var res = unitOfWork.NotificationRepository
                .Search(item => item.ReceiverId == loginUserId)
                .Take(MAX_RECORD)
                .OrderByDescending(item => item.CreatedDate)
                .ToList();

            var unReadNotifications = unitOfWork.NotificationRepository
                .Search(item => item.ReceiverId == loginUserId && item.IsRead)
                .ToList();

            unReadNotifications = unReadNotifications.Select(item =>
            {
                var model = item;
                model.IsRead = true;

                return model;
            }).ToList();

            try
            {
                if (unReadNotifications != null)
                    await unitOfWork.NotificationRepository.UpdateAsync(unReadNotifications);
            }
            catch
            {

            }

            return ResponseService<object>.OK(res);
        }

        public async Task<ResponseDTO> CreateStaffManagerAdminNotification(NotificationRequestDTO request, RoleEnum role)
        {
            try
            {
                var users = unitOfWork.UserRepository.Search(
                    item => item.IsActive == (int)UserStatusEnum.ACTIVE
                    && (item.UserRoles != null) && item.UserRoles.Count(ur => ur.RoleId == (int)role || ur.RoleId == (int)RoleEnum.ADMIN) > 0
                )
                .Include(item => item.UserRoles)
                .ToList();

                var mapper = new MapService<NotificationRequestDTO, Notifications>();

                var saveList = users.Select(
                    item => BuildNotification(mapper, request, item.Id)
                );

                if (saveList != null)
                    await unitOfWork.NotificationRepository.CreateAsync(saveList);

                return ResponseService<object>.OK(saveList?.Count());
            }
            catch (Exception ex)
            {
                return ResponseService<object>.BadRequest(ex.Message);
            }
        }
    }
}
