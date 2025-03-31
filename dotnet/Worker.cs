using dotnet.Services;

namespace dotnet
{
    public class Worker : BackgroundService
    {
        private readonly Infrastructure.RabbitMQ _rabbitmq;
        private PersonService _personService;

        public Worker(Infrastructure.RabbitMQ rabbitmq, PersonService personService)
        {
            _rabbitmq = rabbitmq;
            _personService = personService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitmq.InitAsync();
            await _rabbitmq.ConsumeAsync(_personService.ProcessMessageAsync, stoppingToken);
        }
    }
}
