using ECommerce.Services.EmailAPI.Data;
using ECommerce.Services.EmailAPI.Extensions;
using ECommerce.Services.EmailAPI.Messaging;
using ECommerce.Services.EmailAPI.Service;
using ECommerce.Services.EmailAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("postgresConnection")));
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseNpgsql(configuration.GetConnectionString("postgresConnection"));

// Add services to the container.
builder.Services.AddSingleton<IEmailService>(new EmailService(optionsBuilder.Options));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce.Services.EmailAPI v1");
    c.RoutePrefix = app.Environment.IsDevelopment()
        ? "swagger"
        : string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigrations(app);
app.UseAzureServiceBusConsumer();

app.Run();

void ApplyMigrations(IHost app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

