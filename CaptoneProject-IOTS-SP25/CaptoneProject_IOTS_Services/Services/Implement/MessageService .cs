using CaptoneProject_IOTS_Repository.Base;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MessageService : IMessageService
    {
        private readonly IRabbitMQUnitOfWork _unitOfWork;

        public MessageService(IRabbitMQUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void SendMessage(string message)
        {
            _unitOfWork.RabbitMQRepository.SendMessage(message);
        }
    }
}
