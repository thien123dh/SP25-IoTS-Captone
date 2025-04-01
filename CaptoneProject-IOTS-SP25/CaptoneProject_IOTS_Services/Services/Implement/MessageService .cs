using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
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
                throw new UnauthorizedAccessException("You don't have permission to access");

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
                    .ThenInclude(u => u.Stores)
                .Include(m => m.Receiver)
                    .ThenInclude(u => u.Stores)
                .ToListAsync();

            var chatList = new List<RecentChatDTO>();

            foreach (var m in recentChats)
            {
                bool isSender = m.CreatedBy == loginUserId;
                var otherUser = isSender ? m.Receiver : m.CreatedByNavigation;

                if (otherUser == null) continue;

                bool isStore = await userServices.CheckUserRole(otherUser.Id, RoleEnum.STORE);

                var storeInfo = otherUser.Stores?.FirstOrDefault();
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
                throw new UnauthorizedAccessException("You don't have permission to access");

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

        public async Task<List<MessageGetBeweenUserDTO>> GetMessagesBetweenUsers(int receiverId)
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null)
                throw new UnauthorizedAccessException("You don't have permission to access");

            var loginUserId = loginUser.Id;

            // Lấy tất cả tin nhắn giữa loginUserId và receiverId
            var messages = await _unitOfWork.MessageRepository.GetAll()
                .Where(m => (m.CreatedBy == loginUserId && m.ReceiverId == receiverId) ||
                            (m.CreatedBy == receiverId && m.ReceiverId == loginUserId))
                .OrderBy(m => m.CreatedDate)
                .Include(m => m.CreatedByNavigation)
                    .ThenInclude(u => u.Stores)
                .Include(m => m.Receiver)
                    .ThenInclude(u => u.Stores)
                .ToListAsync();

            var messageList = new List<MessageGetBeweenUserDTO>();

            if (messages.Any()) // Nếu đã có tin nhắn
            {
                // Duyệt qua tất cả tin nhắn
                foreach (var m in messages)
                {
                    bool isSender = m.CreatedBy == loginUserId;
                    var otherUser = isSender ? m.Receiver : m.CreatedByNavigation;

                    if (otherUser == null) continue;

                    bool isStore = await userServices.CheckUserRole(otherUser.Id, RoleEnum.STORE);
                    var storeInfo = otherUser.Stores?.FirstOrDefault();

                    string displayName = isStore ? storeInfo?.Name : otherUser.Fullname;
                    string imageUrl = isStore ? storeInfo?.ImageUrl : otherUser.ImageURL;

                    messageList.Add(new MessageGetBeweenUserDTO
                    {
                        Id = m.Id,
                        name = displayName ?? "",
                        CreatedBy = m.CreatedBy,
                        ReceiverId = m.ReceiverId,
                        Content = m.Content,
                        CreatedDate = m.CreatedDate,
                        imagUrl = imageUrl ?? ""
                    });
                }
            }
            else
            {
                // Kiểm tra vai trò Store cho receiverId
                var receiver = await _unitOfWork.UserRepository.GetUserById(receiverId);

                if (receiver != null)
                {
                    bool isStore = await userServices.CheckUserRole(receiver.Id, RoleEnum.STORE);

                    // Tiếp tục xử lý với receiver và isStore
                    if (isStore)
                    {
                            messageList.Add(new MessageGetBeweenUserDTO
                            {
                                Id = 0,
                                name = receiver.Stores?.FirstOrDefault().Name ?? "Unknown",
                                CreatedBy = loginUserId,
                                ReceiverId = receiverId, // Đây là StoreId
                                Content = "Chưa có tin nhắn nào.",
                                CreatedDate = DateTime.UtcNow,
                                imagUrl = receiver.Stores?.FirstOrDefault().ImageUrl ?? ""
                            });
                    }
                    else
                    {
                        // Xử lý với User (không phải Store)
                        string displayName = receiver.Fullname ?? "Unknown";
                        string imageUrl = receiver.ImageURL ?? "";

                        messageList.Add(new MessageGetBeweenUserDTO
                        {
                            Id = 0,
                            name = displayName,
                            CreatedBy = loginUserId,
                            ReceiverId = receiverId,
                            Content = "Chưa có tin nhắn nào.",
                            CreatedDate = DateTime.UtcNow,
                            imagUrl = imageUrl
                        });
                    }
                }
            }

            return messageList;
        }
    }
}
