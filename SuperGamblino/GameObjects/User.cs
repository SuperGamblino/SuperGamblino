using DSharpPlus.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino.GameObjects
{
    public class User
    {
        public UInt64 Id { get; set; }
        public int Credits { get; set; }
        public DateTime LastHourlyReward { get; set; }
        public DateTime LastDailyReward { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public DiscordUser DiscordUser { get; set; }
    }
}
