using System.Text;
using dotnet.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace dotnet.Infrastructure
{
    public class RabbitMQ : IDisposable
    {
        private readonly ConnectionFactory _factoryConnection;
        private IConnection? _connection;
        private IChannel? _channel;
        private string _queueName;
        private IPersonService _personService;

        public RabbitMQ(IConfiguration configuration, PersonService personService)  
        {
            _personService = personService;

            _factoryConnection = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"]!,
                UserName = configuration["RabbitMQ:Username"]!,
                Password = configuration["RabbitMQ:Password"]!,
            };

            _queueName = configuration["RabbitMQ:QueueName"]!;
        }

        public async Task Init()
        {
            _connection = await _factoryConnection.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public async Task ConsumeAsync(Func<string, Task> processMessageAsync, CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await _personService.ProcessMessage(message);
                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error to process message: {ex.Message}");
                    await _channel!.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            await _channel!.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken); // Keep thread running
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }
    }
}
