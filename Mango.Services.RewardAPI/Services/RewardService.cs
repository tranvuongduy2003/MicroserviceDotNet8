using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Mango.Services.RewardAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Services;

public class RewardService : IRewardService
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;

    public RewardService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public async Task UpdateRewards(RewardsMessage rewardsMessage)
    {
        try
        {
            Rewards rewards = new()
            {
                UserId = rewardsMessage.UserId,
                OrderId = rewardsMessage.OrderId,
                RewardsActivity = rewardsMessage.RewardsActivity,
                RewardsDate = DateTime.Now,
            };
            await using var _db = new AppDbContext(_dbOptions);
            _db.Rewards.AddAsync(rewards);
            _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
        }
    }
}