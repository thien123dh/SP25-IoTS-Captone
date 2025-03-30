﻿using CaptoneProject_IOTS_BOs.DTO.MessageDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CaptoneProject_IOTS_API.Controllers.MessageController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _chatHub;
        public MessageController(IMessageService messageService, IHubContext<ChatHub> chatHub)
        {
            _messageService = messageService;
            _chatHub = chatHub;
        }

        [HttpGet("recent-chats")]
        public async Task<IActionResult> GetRecentChats()
        {
            var chats = await _messageService.GetRecentChats();
            return Ok(chats);
        }

        [HttpPost("Chat-RabbitMQ")]
        public async Task<IActionResult> CreateChatMessage([FromBody] CreateMessageDTO payload)
        {
            var chats = await _messageService.CreateMessage(payload);
            await _chatHub.Clients.User(payload.ReceiverId.ToString()).SendAsync("ReceiveMessage", chats);
            return Ok(chats);
        }
    }
}
