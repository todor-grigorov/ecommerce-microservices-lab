using ECommerce.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Coupon> Coupons { get; set; }
    }
}
