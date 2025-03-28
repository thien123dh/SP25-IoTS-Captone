using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQModel = RabbitMQ.Client.IModel;
using System.Threading.Tasks;

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

        public RabbitMQRepository(IConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "test_queue",
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
