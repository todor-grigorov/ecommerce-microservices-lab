using ECommerce.Services.RewardsApi.Data;
using ECommerce.Services.RewardsApi.Dto;
using ECommerce.Services.RewardsApi.Models;
using ECommerce.Services.RewardsApi.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.RewardsApi.Service
{
    public class RewardsService : IRewardsService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public RewardsService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task UpdateRewards(RewardsMessageDto rewardsMessageDto)
        {
            try
            {
                Rewards rewards = new Rewards()
                {
                    OrderId = rewardsMessageDto.OrderId,
                    RewardsActivity = rewardsMessageDto.RewardsActivity,
                    UserId = rewardsMessageDto.UserId,
                    RewardsDate = DateTime.Now.ToUniversalTime(),
                };



                await using var db = new AppDbContext(_dbOptions);
                await db.Rewards.AddAsync(rewards);
                await db.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating rewards: {ex.Message}");
            }
        }
    }
}
