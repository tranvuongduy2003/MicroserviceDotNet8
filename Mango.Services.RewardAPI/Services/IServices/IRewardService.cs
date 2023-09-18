using Mango.Services.RewardAPI.Message;

namespace Mango.Services.RewardAPI.Services.IServices;

public interface IRewardService
{
    Task UpdateRewards(RewardsMessage rewardsMessage);
}