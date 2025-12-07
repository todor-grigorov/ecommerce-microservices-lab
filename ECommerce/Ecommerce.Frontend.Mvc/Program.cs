using Ecommerce.Frontend.Mvc.Service;
using Ecommerce.Frontend.Mvc.Service.IService;
using Ecommerce.Frontend.Mvc.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Ecommerce.Frontend.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<ICouponService, CouponService>();
            builder.Services.AddHttpClient<IAuthService, AuthService>();

            StaticDetails.CouponApiBase = builder.Configuration["ServiceUrls:CouponAPI"];
            StaticDetails.IdentityApiBase = builder.Configuration["ServiceUrls:IdentityAPI"];
            StaticDetails.ProductApiBase = builder.Configuration["ServiceUrls:ProductAPI"];

            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(10);
                    options.LoginPath = "/Auth/Login";
                    //options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
