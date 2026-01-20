using ECommerce.Gateway.Api.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var appConfigConn = builder.Configuration["AppConfigConnectionString"];

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(appConfigConn)
               // If labels per environment are used, I need to pick one:
               // .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
               .Select("Routes", builder.Environment.EnvironmentName)
               .Select("GlobalConfiguration", builder.Environment.EnvironmentName);
    });
}

builder.AddAppAuthentication();
builder.Services.AddOcelot();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot();

app.Run();
