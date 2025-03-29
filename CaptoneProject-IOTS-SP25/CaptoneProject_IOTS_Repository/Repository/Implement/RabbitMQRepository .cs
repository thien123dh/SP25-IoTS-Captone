using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQModel = RabbitMQ.Client.IModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public interface IRabbitMQRepository
    {
        void SendMessage(string message);
    }

    public class RabbitMQRepository : IRabbitMQRepository
    {
        private readonly IConnection _connection;
        private readonly RabbitMQModel _channel;
        private readonly string _queueName;

        public RabbitMQRepository(IConfiguration configuration)
        {
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfig["HostName"],
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = rabbitConfig["QueueName"];

            _channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: "test_queue",
                                  basicProperties: null,
                                  body: body);
        }
    }
}
