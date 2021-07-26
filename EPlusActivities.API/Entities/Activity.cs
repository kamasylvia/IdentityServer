using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class Activity
    {
        public Activity(string name)
        {
            Name = name;
        }

        [Key]
        public Guid? Id { get; set; }

        // 整个活动期内抽奖次数限制，0 表示无限制
        public int Limit { get; set; }

        // 每用户每天抽奖限制, 0 表示无限制
        public int DailyLimit { get; set; }

        // 兑换一次抽奖所需积分
        public int RequiredCreditForRedeeming { get; set; }

        [Required]
        public string Name { get; set; }

        // 渠道
        public ChannelCode ChannelCode { get; set; }

        // 活动类型：签到、连续签到、抽奖
        public ActivityCode ActivityCode { get; set; }

        // 抽奖展示类型
        public LotteryDisplay LotteryDisplay { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public IEnumerable<Lottery> LotteryResults { get; set; }

        public IEnumerable<PrizeTier> PrizeTiers { get; set; }

        public IEnumerable<ActivityUser> ActivityUserLinks { get; set; }
    }
}
