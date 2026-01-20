using ECommerce.Services.CouponAPI.Data;
using ECommerce.Services.CouponAPI.Extensions;
using ECommerce.Services.CouponAPI.Services;
using ECommerce.Services.CouponAPI.Services.IService;
using ECommerce.Services.CouponAPI.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("postgresConnection")));

StaticDetails.StripeSecretKey = configuration.GetSection("StripeSettings:SecretKey").Get<string>();

builder.Services.AddScoped<IStripeService, StripeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddSwaggerWithAuthorization();

builder.AddAppAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce.Services.CouponAPI v1");
    c.RoutePrefix = app.Environment.IsDevelopment()
        ? "swagger"
        : string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthentication();

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