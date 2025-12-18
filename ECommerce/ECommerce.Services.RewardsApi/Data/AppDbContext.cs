using ECommerce.Services.RewardsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.RewardsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Rewards> Rewards { get; set; }

    }
}
