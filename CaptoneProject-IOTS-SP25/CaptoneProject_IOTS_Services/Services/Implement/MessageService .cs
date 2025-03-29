using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.RabbitMQDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_Repository.Base;
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
    public class MessageService : IMessageService
    {
        /*private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices userServices;
        public MessageService(IUserServices userServices) 
        {
            this._unitOfWork ??= new UnitOfWork();
            this.userServices = userServices;
        }

        public Task<List<string>> GetMessagesForUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RecentChatDTO>> GetRecentChats()
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null)
                throw new Exception("You don't have permission to access");

            var loginUserId = loginUser.Id;

            // Lấy danh sách UserId đã từng chat với loginUserId
            var userChatList = await _unitOfWork.MessageRepository.GetAll()
                .Where(m => m.CreatedBy == loginUserId || m.ReceiverId == loginUserId)
                .GroupBy(m => m.CreatedBy == loginUserId ? m.ReceiverId : m.CreatedBy)
                .Select(g => g.OrderByDescending(m => m.CreatedDate).First().Id)
                .ToListAsync();

            // Lấy danh sách tin nhắn mới nhất với Include()
            var recentChats = await _unitOfWork.MessageRepository.GetAll()
                .Where(m => userChatList.Contains(m.Id))
                .Include(m => m.CreatedByUserNavigation)
                .Include(m => m.ReceiverUserNavigation)
                .Include(m => m.CreatedByStoreNavigation)
                .Include(m => m.ReceiverStoreNavigation)
                .ToListAsync();

            // Chuyển đổi sang DTO
            var chatList = recentChats.Select(m =>
            {
                bool isSender = m.CreatedBy == loginUserId;
                var otherUserId = isSender ? m.ReceiverId ?? 0 : m.CreatedBy ?? 0;

                var otherUser = isSender ? m.ReceiverUserNavigation : m.CreatedByUserNavigation;
                var otherStore = isSender ? m.ReceiverStoreNavigation : m.CreatedByStoreNavigation;

                return new RecentChatDTO
                {
                    UserId = otherUserId,
                    Username = otherStore?.Name ?? otherUser?.Fullname ?? "Unknown",
                    ImageURL = otherStore?.ImageUrl ?? otherUser?.ImageURL,
                    LastMessage = m.Content,
                    LastMessageTime = m.CreatedDate
                };
            }).ToList();

            return chatList;
        }*/
    }
}
