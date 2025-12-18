using ECommerce.Services.RewardsApi.Dto;

namespace ECommerce.Services.EmailAPI.Service.IService
{
    public interface IRewardsService
    {
        Task UpdateRewards(RewardsMessageDto rewardsMessageDto);
    }
}
