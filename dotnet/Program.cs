using dotnet;
using dotnet.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<dotnet.Infrastructure.RabbitMQ>();
        services.AddSingleton<PersonService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();