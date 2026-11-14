using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dboptions;

        public RewardService(DbContextOptions<AppDbContext> dboptions)
        {
            _dboptions = dboptions;
        }
        
        public async Task UpdateRewards(RewardMessage rewardMessage)
        {
            try
            {
                Rewards reward = new Rewards()
                {
                    OrderId =rewardMessage.OrderId,
                    RewardsActivity = rewardMessage.RewardsActivity,
                    UserId = rewardMessage.UserId,
                    RewardsDate = DateTime.Now
                };

                await using var _db = new AppDbContext(_dboptions);
                await _db.Rewards.AddAsync(reward);
                await _db.SaveChangesAsync();

                
            }
            catch (Exception ex)
            {

                //return false;
            }
        
        }
    }
}
