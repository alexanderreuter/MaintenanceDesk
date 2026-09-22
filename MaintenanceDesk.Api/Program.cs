using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using MaintenanceDesk.Api.Data;
using MaintenanceDesk.Api.Events;
using MaintenanceDesk.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Only set in Azure
if (builder.Configuration["KeyVault:Uri"] is { Length: > 0 } keyVaultUri)
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}

// Only set in Azure.
if (builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"] is { Length: > 0 })
{
    builder.Services.AddOpenTelemetry().UseAzureMonitor();
}

// Add services to the container.

var enumsAsNames = new JsonStringEnumConverter(allowIntegerValues: false);

builder.Services.AddControllers(options =>
    {
        // Every DTO field states [Required] explicitly; the implicit rule only adds a misleading
        // "The dto field is required." next to the real error when a body can't be parsed.
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(enumsAsNames);
        options.AllowInputFormatterExceptionMessages = false;
    });

// The OpenAPI options
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(enumsAsNames));
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("MaintenanceDesk")
    ?? throw new InvalidOperationException("Connection string 'MaintenanceDesk' is not configured.");

// Azure SQL refuses connections while resuming from auto-pause and during maintenance, retry those instead of failing.
builder.Services.AddDbContext<MaintenanceDeskDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(
        maxRetryCount: 6,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: [-2])));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<MaintenanceRequestService>();

// Only set in Azure, without it events go nowhere, so local runs and tests need no Service Bus.
if (builder.Configuration["ServiceBus:Namespace"] is { Length: > 0 } serviceBusNamespace)
{
    var queueName = builder.Configuration["ServiceBus:QueueName"] ?? "maintenance-request-events";

    builder.Services.AddSingleton(_ => new ServiceBusClient(serviceBusNamespace, new DefaultAzureCredential()));
    builder.Services.AddSingleton(services => services.GetRequiredService<ServiceBusClient>().CreateSender(queueName));
    builder.Services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>();
}
else
{
    builder.Services.AddSingleton<IEventPublisher, NullEventPublisher>();
}

builder.Services.AddHealthChecks()
    .AddDbContextCheck<MaintenanceDeskDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
