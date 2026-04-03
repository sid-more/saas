using EcommerceSaaS.API.Extensions;
using EcommerceSaaS.API.Filters;
using EcommerceSaaS.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/ecommerce-saas-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelStateAttribute>();
});
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Migrate and seed database
await app.MigrateAndSeedDatabaseAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcommerceSaaS API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseApplicationMiddleware();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
