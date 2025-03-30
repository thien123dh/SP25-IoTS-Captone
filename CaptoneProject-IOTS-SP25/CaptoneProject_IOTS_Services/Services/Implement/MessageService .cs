using CaptoneProject_IOTS_BOs.DTO.MessageDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.RabbitMQDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
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
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices userServices;
        public MessageService(IUserServices userServices)
        {
            this._unitOfWork ??= new UnitOfWork();
         
            this.userServices = userServices;
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

            var recentChats = await _unitOfWork.MessageRepository.GetAll()
                .Where(m => userChatList.Contains(m.Id))
                .Include(m => m.CreatedByNavigation)
                    .ThenInclude(u => u.Stores) // Include danh sách Stores từ User
                .Include(m => m.Receiver)
                    .ThenInclude(u => u.Stores) // Include danh sách Stores từ User
                .ToListAsync();

            var chatList = new List<RecentChatDTO>();

            foreach (var m in recentChats)
            {
                bool isSender = m.CreatedBy == loginUserId;
                var otherUser = isSender ? m.Receiver : m.CreatedByNavigation;

                if (otherUser == null) continue;

                // Kiểm tra user có phải Store không
                bool isStore = await userServices.CheckUserRole(otherUser.Id, RoleEnum.STORE);

                // Nếu là Store, lấy thông tin Store, nếu không lấy thông tin User
                var storeInfo = otherUser.Stores?.FirstOrDefault(); // Lấy store đầu tiên của user (nếu có)
                string displayName = isStore ? storeInfo?.Name : otherUser.Fullname;
                string imageUrl = isStore ? storeInfo?.ImageUrl : otherUser.ImageURL;

                chatList.Add(new RecentChatDTO
                {
                    UserId = otherUser.Id,
                    Username = displayName ?? "Unknown",
                    ImageURL = imageUrl ?? "",
                    LastMessage = m.Content,
                    LastMessageTime = m.CreatedDate
                });
            }
            return chatList.OrderByDescending(c => c.LastMessageTime).ToList();
        }

        public async Task<MessageDTO> CreateMessage(CreateMessageDTO dto)
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null)
                throw new Exception("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var sender = await _unitOfWork.UserRepository.GetByIdAsync(loginUserId);
            var receiver = await _unitOfWork.UserRepository.GetByIdAsync(dto.ReceiverId);

            if (sender == null || receiver == null)
                throw new Exception("Sender or Receiver does not exist.");

            var newMessage = new Message
            {
                Content = dto.Content,
                CreatedBy = loginUserId,
                ReceiverId = dto.ReceiverId,
                CreatedDate = DateTime.Now,
                Status = 1
            };

            _unitOfWork.MessageRepository.Create(newMessage);

            return new MessageDTO
            {
                Id = newMessage.Id,
                Content = newMessage.Content,
                CreatedBy = newMessage.CreatedBy,
                ReceiverId = newMessage.ReceiverId,
                CreatedDate = newMessage.CreatedDate
            };
        }

    }
}
