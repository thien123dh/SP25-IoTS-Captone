using CaptoneProject_IOTS_BOs;
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
using MimeKit;
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
        public async Task<GenericResponseDTO<List<RecentChatDTO>>> GetRecentChats()
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null)
                return ResponseService<List<RecentChatDTO>>.Unauthorize("You don't have permission to access");

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
            return new GenericResponseDTO<List<RecentChatDTO>>()
            {
                Data = chatList.OrderByDescending(c => c.LastMessageTime).ToList(),
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<GenericResponseDTO<MessageDTO>> CreateMessage(CreateMessageDTO dto)
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null)
                return ResponseService<MessageDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var sender = await _unitOfWork.UserRepository.GetByIdAsync(loginUserId);
            var receiver = await _unitOfWork.UserRepository.GetByIdAsync(dto.ReceiverId);

            if (sender == null || receiver == null)
                throw new Exception("Sender or Receiver does not exist.");

            bool isLoginUserStore = await userServices.CheckUserRole(loginUserId, RoleEnum.STORE);
            bool isReceiverStore = await userServices.CheckUserRole(dto.ReceiverId, RoleEnum.STORE);

            if (isLoginUserStore == isReceiverStore)
            {
                return ResponseService<MessageDTO>.BadRequest("Both users cannot be stores or both be normal users.");
            }

            var newMessage = new Message
            {
                Content = dto.Content,
                CreatedBy = loginUserId,
                ReceiverId = dto.ReceiverId,
                CreatedDate = DateTime.Now,
                Status = 1
            };

            _unitOfWork.MessageRepository.Create(newMessage);

            var message = new MessageDTO
            {
                Id = newMessage.Id,
                Content = newMessage.Content,
                CreatedBy = newMessage.CreatedBy,
                ReceiverId = newMessage.ReceiverId,
                CreatedDate = newMessage.CreatedDate
            };

            return new GenericResponseDTO<MessageDTO>()
            {
                Data = message,
                Message = "Success",
                IsSuccess = true
            };
        }


        public async Task<GenericResponseDTO<List<MessageGetBeweenUserDTO>>> GetMessagesBetweenUsers(int receiverId)
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null)
                return ResponseService<List<MessageGetBeweenUserDTO>>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var receiver = await _unitOfWork.UserRepository.GetUserById(receiverId);
            if (receiver == null)
                return ResponseService<List<MessageGetBeweenUserDTO>>.NotFound("Receiver not found");

            bool isLoginUserStore = await userServices.CheckUserRole(loginUserId, RoleEnum.STORE);
            bool isReceiverStore = await userServices.CheckUserRole(receiverId, RoleEnum.STORE);

            if (isLoginUserStore == isReceiverStore)
            {
                return ResponseService<List<MessageGetBeweenUserDTO>>.BadRequest("Both users cannot be stores or both be normal users.");
            }

            var messages = await _unitOfWork.MessageRepository.GetAll()
                .Where(m => (m.CreatedBy == loginUserId && m.ReceiverId == receiverId) ||
                            (m.CreatedBy == receiverId && m.ReceiverId == loginUserId))
                .OrderByDescending(m => m.CreatedDate)
                .Include(m => m.CreatedByNavigation)
                    .ThenInclude(u => u.Stores)
                .Include(m => m.Receiver)
                    .ThenInclude(u => u.Stores)
                .ToListAsync();

            var messageList = new List<MessageGetBeweenUserDTO>();

            if (messages.Any())
            {
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
                string displayName = isReceiverStore ? receiver.Stores?.FirstOrDefault()?.Name ?? "Unknown" : receiver.Fullname ?? "Unknown";
                string imageUrl = isReceiverStore ? receiver.Stores?.FirstOrDefault()?.ImageUrl ?? "" : receiver.ImageURL ?? "";

                messageList.Add(new MessageGetBeweenUserDTO
                {
                    Id = 0,
                    name = displayName,
                    CreatedBy = loginUserId,
                    ReceiverId = receiverId,
                    Content = "",
                    CreatedDate = DateTime.UtcNow,
                    imagUrl = imageUrl
                });
            }

            return new GenericResponseDTO<List<MessageGetBeweenUserDTO>>()
            {
                Data = messageList,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<GenericResponseDTO<bool>> RevokeMessage(int messageId)
        {
            var loginUser = userServices.GetLoginUser();
            if (loginUser == null)
                return ResponseService<bool>.Unauthorize("You don't have permission to access");

            var message = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
            if (message == null)
                return ResponseService<bool>.NotFound("Message not found");

            // Chỉ cho phép người gửi thu hồi tin nhắn
            if (message.CreatedBy != loginUser.Id)
                return ResponseService<bool>.BadRequest("You can only revoke your own messages");

            // Cập nhật trạng thái thành 2 (Thu hồi)
            message.Status = 2;
            _unitOfWork.MessageRepository.Update(message);

            return new GenericResponseDTO<bool>()
            {
                Data = true,
                Message = "Message revoked successfully",
                IsSuccess = true
            };
        }

    }
}
