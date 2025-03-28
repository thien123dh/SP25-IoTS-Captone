using CaptoneProject_IOTS_Repository.Repository.Implement;
using RabbitMQ.Client;
using RabbitMQModel = RabbitMQ.Client.IModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Base
{
    public interface IRabbitMQUnitOfWork : IDisposable
    {
        IRabbitMQRepository RabbitMQRepository { get; }
    }

    public class RabbitMQUnitOfWork : IRabbitMQUnitOfWork
    {
        private readonly IConnection _connection;
        private readonly RabbitMQModel _channel;
        private IRabbitMQRepository _rabbitMQRepository;

        public RabbitMQUnitOfWork()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Khai báo hàng đợi chung ở đây để tránh bị gọi lại nhiều lần
            _channel.QueueDeclare(queue: "test_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public IRabbitMQRepository RabbitMQRepository => _rabbitMQRepository ??= new RabbitMQRepository(_connection);

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
