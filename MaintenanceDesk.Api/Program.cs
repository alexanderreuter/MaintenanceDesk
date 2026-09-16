using System.Text.Json.Serialization;
using MaintenanceDesk.Api.Data;
using MaintenanceDesk.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<MaintenanceRequestService>();

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
