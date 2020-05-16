﻿using System;

 namespace SuperGamblino.Core.Entities
{
    public class User
    {
        public ulong Id { get; set; }
        public int Credits { get; set; }
        public DateTime? LastHourlyReward { get; set; }
        public DateTime? LastDailyReward { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
    }
}