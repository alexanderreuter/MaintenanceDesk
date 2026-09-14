using MaintenanceDesk.Api.Data;
using MaintenanceDesk.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("MaintenanceDesk")
    ?? throw new InvalidOperationException("Connection string 'MaintenanceDesk' is not configured.");

builder.Services.AddDbContext<MaintenanceDeskDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<MaintenanceRequestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
