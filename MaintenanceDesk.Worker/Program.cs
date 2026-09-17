using Azure.Identity;
using Azure.Messaging.ServiceBus;
using MaintenanceDesk.Worker;
using MaintenanceDesk.Worker.Api;

var builder = Host.CreateApplicationBuilder(args);

var serviceBusNamespace = builder.Configuration["ServiceBus:Namespace"]
    ?? throw new InvalidOperationException("'ServiceBus:Namespace' is not configured.");

var queueName = builder.Configuration["ServiceBus:QueueName"]
    ?? throw new InvalidOperationException("'ServiceBus:QueueName' is not configured.");

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("'Api:BaseUrl' is not configured.");

builder.Services.AddSingleton(_ => new ServiceBusClient(serviceBusNamespace, new DefaultAzureCredential()));

builder.Services.AddSingleton(services => services.GetRequiredService<ServiceBusClient>()
    .CreateProcessor(queueName, new ServiceBusProcessorOptions
    {
        MaxConcurrentCalls = 1,
        AutoCompleteMessages = false,
    }));

builder.Services.AddSingleton(new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
{
    BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/"),
});

builder.Services.AddSingleton<MaintenanceRequestsClient>();

builder.Services.AddHostedService<MaintenanceEventWorker>();

var host = builder.Build();
host.Run();
