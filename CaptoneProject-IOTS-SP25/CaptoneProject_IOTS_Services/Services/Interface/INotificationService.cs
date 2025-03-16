using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.NotificationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface INotificationService
    {
        public Task<ResponseDTO> GetNotifications();
        public Task<ResponseDTO> CreateStaffManagerAdminNotification(NotificationRequestDTO request, RoleEnum role);
        public Task<ResponseDTO> CreateUserNotification(List<NotificationRequestDTO> request);
        public Task<ResponseDTO> CountNotReadNotification();
    }
}
