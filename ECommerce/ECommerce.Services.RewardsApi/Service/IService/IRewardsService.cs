using ECommerce.Services.RewardsApi.Dto;

namespace ECommerce.Services.RewardsApi.Service.IService
{
    public interface IRewardsService
    {
        Task UpdateRewards(RewardsMessageDto rewardsMessageDto);
    }
}
