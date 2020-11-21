using System;
using Dapper.Contrib.Extensions;

namespace SuperGamblino.Core.Entities
{
    [Table("Users")]
    public class User
    {
        public User(ulong userId)
        {
            Id = userId;
        }

        public User()
        {
        }

        [ExplicitKey] public ulong Id { get; set; }

        public int Credits { get; set; }
        public DateTime? LastHourlyReward { get; set; }
        public DateTime? LastDailyReward { get; set; }
        public DateTime? LastVoteReward { get; set; }
        public DateTime? LastWorkReward { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; } = 1;
    }
}