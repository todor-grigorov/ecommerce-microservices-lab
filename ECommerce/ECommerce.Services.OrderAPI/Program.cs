using ECommerce.Integration.MessageBus;
using ECommerce.Services.OrderAPI.Data;
using ECommerce.Services.OrderAPI.Extensions;
using ECommerce.Services.OrderAPI.Service;
using ECommerce.Services.OrderAPI.Service.IService;
using ECommerce.Services.OrderAPI.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("postgresConnection")));

StaticDetails.StripeSecretKey = configuration.GetSection("StripeSettings:SecretKey").Get<string>();

builder.Services.AddHttpClient("Product", c => c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddScoped<IServiceBus, ServiceBus>();
builder.Services.AddScoped<IStripeService, StripeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddSwaggerWithAuthorization();

builder.AddAppAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
ApplyMigrations(app);

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
