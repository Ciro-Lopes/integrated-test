using System.Diagnostics;
using System;
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

        public RabbitMQ(IConfiguration configuration, PersonService personService)  
        {
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
            int retries = 5;
            while (retries > 0)
            {
                try
                {
                    _connection = await _factoryConnection.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();
                    await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    Console.WriteLine($"🎯 Queue '${_queueName}' created with success.");

                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error -> {ex.Message}");
                    Console.WriteLine($"❌ Error to connect on RabbitMQ, trying again... (${ retries} remaining attempts)");
                    retries--;

                    Thread.Sleep(5000); // Wait 5s before trying again
                }
            }

            Console.WriteLine("❌ Unable to connect to RabbitMQ.");
            Environment.Exit(0);
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
                    await processMessageAsync(message);
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
