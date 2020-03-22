using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperGamblino.Commands
{
    class Cooldown
    {
        private readonly Database _database;
        private readonly Messages _messages;
        private readonly Config _config;

        public Cooldown(Database database, Config config, Messages messages)
        {
            _database = database;
            _config = config;
            _messages = messages;
        }

        [Command("cooldown")]
        [Aliases("cd")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            DateTimeResult curLHR = await _database.GetDateTime(command.User.Id, "last_hourly_reward");
            DateTimeResult curLDR = await _database.GetDateTime(command.User.Id, "last_daily_reward");

            List<CooldownObject> cooldownObjects = new List<GameObjects.CooldownObject>();

            if (curLHR.Successful && curLDR.Successful)
            {
                if (curLHR.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLHR.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(1))
                    {
                        cooldownObjects.Add(new CooldownObject("Hourly"));
                    }
                    else
                    {
                        cooldownObjects.Add(new CooldownObject("Hourly", TimeSpan.FromHours(1) - timeSpan));
                    }
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Hourly"));
                }
                if (curLDR.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLHR.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromDays(1))
                    {
                        cooldownObjects.Add(new CooldownObject("Daily"));
                    }
                    else
                    {
                        cooldownObjects.Add(new CooldownObject("Daily", TimeSpan.FromDays(1) - timeSpan));
                    }
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Daily"));
                }

                await _messages.ListCooldown(command, cooldownObjects);
            }
            else
            {
                await _messages.Error(command, "Some problem with DB occured!");
            }
        }
    }
}
