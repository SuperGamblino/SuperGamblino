using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperGamblino.GameObjects
{
    class Work
    {
        public struct Job
        {
            public string Title { get; set; }
            public int Cooldown { get; set; }
            public int Reward { get; set; }
            public int Level { get; set; }
        }
        public static List<Job> Jobs { get; } = new List<Job>
        {
            new Job{ Title = "Janitor", Cooldown = 6, Reward = 30, Level = 1},
            new Job{ Title = "Super Market Assistant", Cooldown = 6, Reward = 45, Level = 4 },
            new Job{ Title = "Junior Soccer Coach", Cooldown = 6, Reward = 65, Level = 7 },
            new Job{ Title = "Kindergarten Teacher", Cooldown = 6, Reward = 90, Level = 10 },
            new Job{ Title = "Substitute Cleaning Personell", Cooldown = 6, Reward = 140, Level = 15},
            new Job{ Title = "Highschool Teacher", Cooldown = 6, Reward = 210, Level = 18},
            new Job{ Title = "Software Developer", Cooldown = 6, Reward = 290, Level = 20},
            new Job{ Title = "Senior Construction Architecht", Cooldown = 6, Reward = 400, Level = 24}
        };

        public static Job GetCurrentJob(int level)
        {
            Job currentJob = Jobs.Where(x => x.Level <= level)
                .OrderByDescending(x => x.Reward)
                .FirstOrDefault();

            return currentJob;
        }
    }
}
