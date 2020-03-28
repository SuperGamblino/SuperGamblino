using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.GameObjects;

namespace SuperGamblino.Commands
{
    internal class Cooldown
    {
        private readonly Config _config;
        private readonly Database _database;
        private readonly Messages _messages;

        public Cooldown(Database database, Config config, Messages messages)
        {
            _database = database;
            _config = config;
            _messages = messages;
        }

        [Command("cooldown")]
        [Aliases("cd")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows the current command cooldowns. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            var curLHR = await _database.GetDateTime(command.User.Id, "last_hourly_reward");
            var curLDR = await _database.GetDateTime(command.User.Id, "last_daily_reward");
            var curLWR = await _database.GetDateTime(command.User.Id, "last_work_reward");

            var cooldownObjects = new List<CooldownObject>();

            if (curLHR.Successful && curLDR.Successful)
            {
                if (curLHR.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLHR.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(1))
                        cooldownObjects.Add(new CooldownObject("Hourly"));
                    else
                        cooldownObjects.Add(new CooldownObject("Hourly", TimeSpan.FromHours(1) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Hourly"));
                }

                if (curLDR.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLDR.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromDays(1))
                        cooldownObjects.Add(new CooldownObject("Daily"));
                    else
                        cooldownObjects.Add(new CooldownObject("Daily", TimeSpan.FromDays(1) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Daily"));
                }

                if (curLWR.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLWR.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(6))
                        cooldownObjects.Add(new CooldownObject("Work"));
                    else
                        cooldownObjects.Add(new CooldownObject("Work", TimeSpan.FromHours(6) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Work"));
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